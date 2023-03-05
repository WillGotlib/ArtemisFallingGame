package backend

import (
	pb "artemisFallingServer/proto"
	"sync"
)

type ActionGroup struct {
	Sender    *Client
	Game      *Game
	received  int
	Responses []*Action
	mu        sync.RWMutex
}

func NewActonGroup(game *Game, sender *Client, nRequests int) *ActionGroup {
	return &ActionGroup{
		Sender:    sender,
		Game:      game,
		Responses: make([]*Action, nRequests),
	}
}

func (g *ActionGroup) Collect(action *Action) bool {
	g.mu.Lock()
	g.received++
	g.Responses[action.index] = action
	g.mu.Unlock()

	return g.received == len(g.Responses)
}

type Action struct {
	index    int
	Request  *pb.StreamAction
	Response *pb.StreamAction
	Group    *ActionGroup
	Priority bool
}

func NewAction(index int, request *pb.StreamAction, group *ActionGroup, priority bool) *Action {
	return &Action{
		index:    index,
		Request:  request,
		Group:    group,
		Priority: priority,
	}
}

func (g *ActionGroup) GetActions() []*pb.StreamAction {
	actions := make([]*pb.StreamAction, len(g.Responses))
	for i, action := range g.Responses {
		actions[i] = action.Response
	}
	return actions
}

func (a *Action) Game() *Game {
	return a.Group.Game
}

func (a *Action) Client() *Client {
	return a.Group.Sender
}

func (a *Action) AddResponse(response *pb.StreamAction) *Action {
	a.Response = response
	return a
}
