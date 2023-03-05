package main

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"context"
	"fmt"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"github.com/pion/webrtc/v3"
	"google.golang.org/protobuf/proto"
	"net/http"
	"sync"
	"time"
)

const (
	EmptySessionTimeout = 10 * time.Second // 10 seconds should be plenty of time for someone to join
	clientTimeout       = 2 * time.Minute
)

var server *GameServer

// GameServer is used to stream game information with clients.
type GameServer struct {
	ChangeChannel chan backend.Change
	games         map[string]*backend.Game
	gameTimeouts  map[string]context.CancelFunc
	sessionUsers  map[string][]*backend.Client
	clients       map[backend.Token]*backend.Client
	mu            sync.RWMutex
}

// NewGameServer constructs a new game server struct.
func NewGameServer() *GameServer {
	if server != nil {
		server.Stop()
	}

	server = &GameServer{
		games:         make(map[string]*backend.Game),
		gameTimeouts:  make(map[string]context.CancelFunc),
		clients:       make(map[backend.Token]*backend.Client),
		ChangeChannel: make(chan backend.Change, 10),
		sessionUsers:  make(map[string][]*backend.Client),
	}
	server.watchChanges()
	server.watchTimeout()
	return server
}

func (s *GameServer) Stop() {
	for id := range s.games {
		multiLogger.Printf("Stopping \"%s\"\n", id)
		s.removeSession(id, true)
	}
	server = nil
}

func (s *GameServer) addClient(c *backend.Client) {
	s.mu.Lock()
	s.clients[c.Id] = c
	s.sessionUsers[c.Session.GameId] = append(s.sessionUsers[c.Session.GameId], c)
	s.mu.Unlock()
}

func (s *GameServer) removeClient(id backend.Token, message string) {
	multiLogger.Printf("%s - removing client", id)

	c := s.clients[id]
	c.Done(message) // close connection if it's still open
	session := c.Session
	session.RemoveClientsEntities(c)
	s.mu.Lock()
	delete(s.clients, id)
	users, ok := s.sessionUsers[session.GameId]
	if !ok {
		s.mu.Unlock()
		return
	}
	serverUsers := make([]*backend.Client, len(users)-1, maxClients)
	count := 0
	for _, i := range users {
		if i == c {
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
		s.sessionUsers[id] = make([]*backend.Client, 0, maxClients)
		s.games[id].Start()
		multiLogger.Println("starting new session", id)
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

	s.mu.Lock()
	for _, c := range s.sessionUsers[id] {
		c.Done("session has shut down")
		delete(s.clients, c.Id)
	}
	session := s.games[id]
	delete(s.gameTimeouts, id)
	delete(s.games, id)
	delete(s.sessionUsers, id)
	s.mu.Unlock()

	session.Stop()
	multiLogger.Println("closing session", id)
}

func (s *GameServer) getClientFromContext(c echo.Context) (*backend.Client, error) {
	tokenRaw := c.Request().Header.Get("Token")
	if len(tokenRaw) == 0 {
		return nil, echo.NewHTTPError(http.StatusBadRequest, "no token provided")
	}
	uid, err := uuid.Parse(tokenRaw)
	if err != nil {
		return nil, echo.NewHTTPError(http.StatusInternalServerError, fmt.Sprintf("cant parse token \"%s\"", tokenRaw))
	}
	s.mu.RLock()
	currentClient, ok := s.clients[backend.Token(uid)]
	s.mu.RUnlock()
	if !ok {
		return nil, echo.NewHTTPError(http.StatusUnauthorized, "token not recognized")
	}
	return currentClient, nil
}

func (s *GameServer) parseMessage(dcLabel string, client *backend.Client) func(msg webrtc.DataChannelMessage) {
	if dcLabel != "priority" && dcLabel != "fast" {
		return func(msg webrtc.DataChannelMessage) {
		}
	}

	priority := dcLabel == "priority"

	return func(msg webrtc.DataChannelMessage) {
		client.LastMessage = time.Now()

		req := new(pb.Request)
		err := proto.Unmarshal(msg.Data, req)
		if err != nil {
			multiLogger.Debug("cant parse ", msg)
		}

		requests := req.GetRequests()
		group := backend.NewActonGroup(client.Session, client, len(requests))
		for i, request := range requests {
			s.handleRequests(backend.NewAction(i, request, group, priority))
		}
	}
}

func (s *GameServer) watchTimeout() {
	timeoutTicker := time.NewTicker(1 * time.Minute)
	go func() {
		for {
			for _, client := range s.clients {
				if time.Since(client.LastMessage) > clientTimeout {
					s.removeClient(client.Id, "you have been timed out")
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

	m := &pb.Response{Responses: group.GetActions()}
	message, err := proto.Marshal(m)
	if err != nil {
		multiLogger.Errorf("send failed: %s", err.Error())
		return
	}

	s.mu.Lock()
	for id, currentClient := range s.clients {
		if currentClient.FastChannel == nil || currentClient.PriorityChannel == nil {
			continue
		}
		if group.Sender != nil && (currentClient.Session.GameId != group.Sender.Session.GameId || // if client is nil then the message will be sent to all users in all sessions
			currentClient.Id == group.Sender.Id) {
			continue
		}

		var channel *webrtc.DataChannel
		if resp.Priority {
			channel = currentClient.PriorityChannel
		} else {
			channel = currentClient.FastChannel
		}

		if err := channel.Send(message); err != nil {
			multiLogger.Printf("%s - broadcast error %v", id, err)
			s.removeClient(currentClient.Id, "failed to broadcast message")
			continue
		}
	}
	s.mu.Unlock()
}
