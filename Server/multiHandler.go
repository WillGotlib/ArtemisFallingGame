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

//todo object pooling

const (
	EmptySessionTimeout = 10 * time.Second // 10 seconds should be plenty of time for someone to join
	clientTimeout       = 30 * time.Second
)

var server *GameServer

// GameServer is used to stream game information with clients.
type GameServer struct {
	ChangeChannel chan backend.Change
	games         map[string]*backend.Game
	gameTimeouts  map[string]context.CancelFunc
	sessionUsers  map[string][]*backend.Client
	clients       map[backend.Token]*backend.Client

	gamesMu    sync.RWMutex
	timeoutsMu sync.RWMutex
	sessionsMu sync.RWMutex
	clientsMu  sync.RWMutex
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
	wg := sync.WaitGroup{}
	s.gamesMu.RLock()
	wg.Add(len(s.games))
	for id := range s.games {
		multiLogger.Printf("Stopping \"%s\"\n", id)
		go func(id string) {
			s.removeSession(id, true)
			wg.Done()
		}(id)
	}
	s.gamesMu.RUnlock()
	wg.Wait()
	time.Sleep(100 * time.Millisecond)
	server = nil
}

func (s *GameServer) addClient(c *backend.Client) {
	s.clientsMu.Lock()
	s.clients[c.Id] = c
	s.clientsMu.Unlock()

	s.sessionsMu.Lock()
	s.sessionUsers[c.Session.GameId] = append(s.sessionUsers[c.Session.GameId], c)
	s.sessionsMu.Unlock()
}

func (s *GameServer) removeClient(id backend.Token, message string) {
	s.clientsMu.RLock()
	c := s.clients[id]
	s.clientsMu.RUnlock()

	if c == nil {
		multiLogger.Debugf("atempting to remove an already deleted client %s", id)
		return
	}

	multiLogger.Infof("%s - removing client", id)
	c.Done(message) // close connection if it's still open
	session := c.Session
	session.RemoveClientsEntities(c)
	s.clientsMu.Lock()
	delete(s.clients, id)
	s.clientsMu.Unlock()

	s.sessionsMu.RLock()
	users, ok := s.sessionUsers[session.GameId]
	s.sessionsMu.RUnlock()
	if !ok {
		return
	}

	nUsers := len(users) - 1
	if nUsers < 0 {
		nUsers = 0
	}

	serverUsers := make([]*backend.Client, nUsers, maxClients)
	count := 0
	for _, i := range users {
		if i == c {
			continue
		}
		serverUsers[count] = i
		count++
	}

	s.sessionsMu.Lock()
	s.sessionUsers[session.GameId] = serverUsers
	s.sessionsMu.Unlock()
	if len(serverUsers) == 0 {
		s.removeSession(session.GameId, false)
	}
}

// makeSession takes a game id and returns true if it created a new session
func (s *GameServer) makeSession(id string) bool {
	s.gamesMu.RLock()
	_, ok := s.games[id]
	s.gamesMu.RUnlock()
	if !ok {
		s.gamesMu.Lock()
		s.games[id] = backend.NewGame(s.ChangeChannel, id)
		s.games[id].Start()
		s.gamesMu.Unlock()

		s.sessionsMu.Lock()
		s.sessionUsers[id] = make([]*backend.Client, 0, maxClients)
		s.sessionsMu.Unlock()

		multiLogger.Println("starting new session", id)
	}
	return !ok
}

func (s *GameServer) removeSession(id string, immediate bool) {
	if !immediate {
		ctx, cancel := context.WithCancel(context.Background())
		timer := time.NewTimer(EmptySessionTimeout)

		s.timeoutsMu.Lock()
		s.gameTimeouts[id] = cancel
		s.timeoutsMu.Unlock()

		select {
		case <-timer.C:
		case <-ctx.Done():
			timer.Stop()
			return
		}
	}

	s.sessionsMu.Lock()
	s.clientsMu.Lock()
	for _, c := range s.sessionUsers[id] {
		go c.Done("session has shut down")
		delete(s.clients, c.Id)
	}
	delete(s.sessionUsers, id)
	s.clientsMu.Unlock()
	s.sessionsMu.Unlock()

	s.gamesMu.Lock()
	if _, ok := s.games[id]; ok {
		s.games[id].Stop()
	}
	delete(s.games, id)
	s.gamesMu.Unlock()

	s.timeoutsMu.Lock()
	delete(s.gameTimeouts, id)
	s.timeoutsMu.Unlock()

	multiLogger.Println("closing session", id)
}

func (s *GameServer) getClientFromContext(c echo.Context) (*backend.Client, error) {
	tokenRaw := c.Request().Header.Get("Authorization")
	if len(tokenRaw) == 0 {
		return nil, echo.NewHTTPError(http.StatusBadRequest, "no token provided")
	}
	uid, err := uuid.Parse(tokenRaw)
	if err != nil {
		return nil, echo.NewHTTPError(http.StatusInternalServerError, fmt.Sprintf("cant parse token \"%s\"", tokenRaw))
	}
	s.clientsMu.RLock()
	currentClient, ok := s.clients[backend.Token(uid)]
	s.clientsMu.RUnlock()
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
	timeoutTicker := time.NewTicker(5 * time.Second)
	go func() {
		for {
			s.clientsMu.RLock()
			for _, client := range s.clients {
				if time.Since(client.LastMessage) > clientTimeout {
					go s.removeClient(client.Id, "you have been timed out")
				}
			}
			s.clientsMu.RUnlock()

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

	s.clientsMu.RLock()
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
	s.clientsMu.RUnlock()
}
