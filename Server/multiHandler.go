package main

//todo
// empty sersions get deleted
// have server ping everyone to find disconnects // keepalive
//todo give players an index when they join
//todo track rotation

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"context"
	"errors"
	"io"
	"log"
	"sync"
	"time"

	"google.golang.org/grpc/metadata"
)

const (
	EmptySessionTimeout = 10 * time.Second // 10 seconds should be plenty of time for someone to join
	clientTimeout       = 2 * time.Minute
)

// GameServer is used to stream game information with clients.
type GameServer struct {
	pb.UnimplementedGameServer
	ChangeChannel chan backend.Change
	games         map[string]*backend.Game
	gameTimeouts  map[string]context.CancelFunc
	sessionUsers  map[string][]backend.Token
	clients       map[backend.Token]*backend.Client
	mu            sync.RWMutex
}

// NewGameServer constructs a new game server struct.
func NewGameServer() *GameServer {
	server := &GameServer{
		games:         make(map[string]*backend.Game),
		gameTimeouts:  make(map[string]context.CancelFunc),
		clients:       make(map[backend.Token]*backend.Client),
		ChangeChannel: make(chan backend.Change, 10),
		sessionUsers:  make(map[string][]backend.Token),
	}
	server.watchChanges()
	server.watchTimeout()
	return server
}

func (s *GameServer) Stop() {
	for id := range s.games {
		log.Printf("Stopping \"%s\"\n", id)
		s.removeSession(id, true)
	}
}

func (s *GameServer) addClient(c *backend.Client) {
	s.mu.Lock()
	s.clients[c.Id] = c
	s.sessionUsers[c.Session.GameId] = append(s.sessionUsers[c.Session.GameId], c.Id)
	s.mu.Unlock()
}

func (s *GameServer) removeClient(id backend.Token) {
	log.Printf("%s - removing client", id)

	c := s.clients[id]
	session := c.Session
	session.RemoveClientsEntities(c)
	s.mu.Lock()
	delete(s.clients, id)
	users, ok := s.sessionUsers[session.GameId]
	if !ok {
		s.mu.Unlock()
		return
	}
	serverUsers := make([]backend.Token, len(users)-1, maxClients)
	count := 0
	for _, i := range users {
		if i == c.Id {
			continue
		}
		serverUsers[count] = i
		count++
	}
	s.sessionUsers[session.GameId] = serverUsers
	s.mu.Unlock()
	if len(serverUsers) == 0 {
		s.removeSession(session.GameId, false)
	}
}

// makeSession takes a game id and returns true if it created a new session
func (s *GameServer) makeSession(id string) bool {
	s.mu.Lock()
	_, ok := s.games[id]
	if !ok {
		s.games[id] = backend.NewGame(s.ChangeChannel, id)
		s.sessionUsers[id] = make([]backend.Token, 0, maxClients)
		s.games[id].Start()
		log.Println("starting new session", id)
	}
	s.mu.Unlock()
	return !ok
}

func (s *GameServer) removeSession(id string, immediate bool) {
	if !immediate {
		ctx, cancel := context.WithCancel(context.Background())
		timer := time.NewTimer(EmptySessionTimeout)

		s.gameTimeouts[id] = cancel

		select {
		case <-timer.C:
		case <-ctx.Done():
			timer.Stop()
			return
		}
	}

	for _, c := range s.sessionUsers[id] {
		s.clients[c].Done <- errors.New("session has shut down")
	}

	s.mu.Lock()
	session := s.games[id]
	delete(s.gameTimeouts, id)
	delete(s.games, id)
	delete(s.sessionUsers, id)
	s.mu.Unlock()

	session.Stop()
	log.Println("closing session", id)
}

func (s *GameServer) getClientFromContext(ctx context.Context) (*backend.Client, error) {
	headers, ok := metadata.FromIncomingContext(ctx)
	tokenRaw := headers["authorization"]
	if len(tokenRaw) == 0 {
		return nil, errors.New("no token provided")
	}
	uid, ok := ParseUUID(tokenRaw[0])
	if !ok {
		return nil, errors.New("cannot parse token")
	}
	s.mu.RLock()
	currentClient, ok := s.clients[backend.Token(uid)]
	s.mu.RUnlock()
	if !ok {
		return nil, errors.New("token not recognized")
	}
	return currentClient, nil
}

// Stream is the main loop for dealing with individual players.
func (s *GameServer) Stream(srv pb.Game_StreamServer) error {
	ctx := srv.Context()

	currentClient, err := s.getClientFromContext(ctx)
	if err != nil {
		return err
	}
	if currentClient.StreamServer != nil {
		return errors.New("stream already active")
	}
	currentClient.StreamServer = srv
	game := currentClient.Session
	if cancelTimeout, ok := s.gameTimeouts[game.GameId]; ok {
		cancelTimeout() // don't turn off the session someone is joining
	}

	defer s.removeClient(currentClient.Id)
	// Wait for stream requests.
	var doneError error

	go func() {
		var req *pb.Request
		for {
			req, err = srv.Recv()
			if err == io.EOF {
				currentClient.Done <- nil
				return
			}
			if err != nil {
				log.Printf("receive error %v", err)
				currentClient.Done <- errors.New("failed to receive request")
				return
			}
			//log.Printf("got message %+v", req)
			currentClient.LastMessage = time.Now()
			requests := req.GetRequests()
			group := backend.NewActonGroup(game, currentClient, len(requests))
			for i, request := range requests { //todo
				s.handleRequests(backend.NewAction(i, request, group))
			}
		}
	}()
	select {
	case doneError = <-currentClient.Done:
	case <-ctx.Done():
		doneError = ctx.Err()
	}

	if doneError != nil {
		log.Printf(`stream done with error "%v"`, doneError)
	}

	return doneError
}

func (s *GameServer) List(ctx context.Context, req *pb.SessionRequest) (*pb.SessionList, error) {
	servers := make([]*pb.Server, 0, len(s.games))
	for u := range s.games {
		servers = append(servers, &pb.Server{
			Id:     u,
			Online: uint32(len(s.sessionUsers[u])),
			Max:    uint32(maxClients),
		})
	}
	return &pb.SessionList{
		Servers: servers,
	}, nil
}

func (s *GameServer) Connect(ctx context.Context, req *pb.ConnectRequest) (*pb.ConnectResponse, error) {
	sessionId := req.GetSession()
	if len(sessionId) < 3 || len(sessionId) > 25 {
		return nil, errors.New("invalid sessionId Provided")
	}
	log.Println("Incoming connection to", req.Session)

	if s.makeSession(sessionId) {
		go s.removeSession(sessionId, false) // remove after timeout if new session
	}
	sessionUsers := s.sessionUsers[sessionId]
	if len(sessionUsers) >= maxClients {
		return nil, errors.New("the server is full")
	}

	game := s.games[sessionId]
	entities := game.GetProtoEntities()

	client := backend.NewClient(game)
	s.addClient(client)

	return &pb.ConnectResponse{
		Token:    client.Id.String(),
		Entities: entities,
	}, nil
}

func (s *GameServer) watchTimeout() {
	timeoutTicker := time.NewTicker(1 * time.Minute)
	go func() {
		for {
			for _, client := range s.clients {
				if time.Since(client.LastMessage) > clientTimeout {
					client.Done <- errors.New("you have been timed out")
					return
				}
			}
			<-timeoutTicker.C
		}
	}()
}

func (s *GameServer) broadcast(resp *backend.Action) {
	group := resp.Group
	if !group.Collect(resp) {
		return // not all messages collected yet
	}

	s.mu.Lock()
	for id, currentClient := range s.clients {
		if currentClient.StreamServer == nil {
			continue
		}
		if group.Sender != nil && (currentClient.Session.GameId != group.Sender.Session.GameId || // if client is nil then the message will be sent to all users in all sessions
			currentClient.Id == group.Sender.Id) {
			continue
		}

		if err := currentClient.StreamServer.Send(&pb.Response{Responses: group.GetActions()}); err != nil {
			log.Printf("%s - broadcast error %v", id, err)
			currentClient.Done <- errors.New("failed to broadcast message")
			continue
		}
	}
	s.mu.Unlock()
}
