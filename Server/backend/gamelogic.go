package backend

import (
	pb "artemisFallingServer/proto"
	"encoding/hex"
	"github.com/google/uuid"
	"strings"
	"sync"
)

type Game struct {
	Entities      map[uuid.UUID]*Entity
	Mu            sync.RWMutex
	ActionChannel chan GameAction
	ChangeChannel chan Change
	done          chan interface{}
	GameId        string

	ownedEntities map[Token][]uuid.UUID
}

// NewGame constructs a new Game struct.
func NewGame(ChangeChannel chan Change, sessionId string) *Game {
	game := Game{
		Entities:      make(map[uuid.UUID]*Entity),
		ActionChannel: make(chan GameAction, 10),
		ChangeChannel: ChangeChannel,
		done:          make(chan interface{}),

		ownedEntities: make(map[Token][]uuid.UUID),
		GameId:        sessionId,
	}
	return &game
}

func (g *Game) Start() {
	go g.watchActions()
}

func (g *Game) Stop() {
	close(g.done)
}

func (g *Game) watchActions() {
	for {
		select {
		case action := <-g.ActionChannel:
			g.ChangeChannel <- action.Perform(g)
		case <-g.done:
			return
		}
	}
}

// AddEntity adds an entity to the game.
func (g *Game) AddEntity(entity *Entity, action *Action, RemoveOnDisconnect bool) {
	g.ActionChannel <- &AddAction{
		baseAction:         g.getBaseAction(entity.ID, action),
		Entity:             entity,
		RemoveOnDisconnect: RemoveOnDisconnect,
	}
}

func (g *Game) GetProtoEntities() []*pb.Entity {
	g.Mu.RLock()
	entities := make([]*pb.Entity, 0, len(g.Entities))
	for _, entity := range g.Entities {
		entities = append(entities, entity.ToProto())
	}
	g.Mu.RUnlock()
	return entities
}

func (g *Game) MoveEntity(id uuid.UUID, action *Action, position *Coordinate, rotation *Rotation) {
	g.ActionChannel <- &MoveAction{
		baseAction: g.getBaseAction(id, action),
		Position:   position,
		Rotation:   rotation,
	}
}

// RemoveEntity removes an entity from the game.
func (g *Game) RemoveEntity(id uuid.UUID, action *Action) {
	g.ActionChannel <- &RemoveAction{
		baseAction: g.getBaseAction(id, action),
	}
}

// RemoveClientsEntities clears out all entities belonging to a user
func (g *Game) RemoveClientsEntities(client *Client) {
	g.Mu.RLock()
	entities, ok := g.ownedEntities[client.Id]
	g.Mu.RUnlock()
	if !ok {
		return
	}
	group := NewActonGroup(g, NilClient(client), len(entities))
	for i, eid := range entities {
		g.RemoveEntity(eid, NewAction(i, nil, group, true))
	}
	g.Mu.Lock()
	delete(g.ownedEntities, client.Id)
	g.Mu.Unlock()
}

// UpdateEntity sends the entities entire information.
func (g *Game) UpdateEntity(entity *Entity, action *Action) {
	g.ActionChannel <- &UpdateAction{
		baseAction: g.getBaseAction(entity.ID, action),
		Entity:     entity,
	}
}

type Event interface {
	EntityID() uuid.UUID
	EntityIDBytes() []byte
	GameID() string
}

type baseEvent struct {
	action *Action
	id     uuid.UUID
}

func (b baseEvent) EntityID() uuid.UUID {
	return b.id
}
func (b baseEvent) EntityIDBytes() []byte {
	u := strings.ReplaceAll(b.id.String(), "-", "")
	id, _ := hex.DecodeString(u)
	return id
}

func (b baseEvent) GameID() string {
	return b.Client().Session.GameId
}
func (b baseEvent) UserID() Token {
	return b.Client().Id
}
func (b baseEvent) Client() *Client {
	return b.action.Client()
}
func (b baseEvent) Action() *Action {
	return b.action
}
