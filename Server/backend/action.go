package backend

import (
	"fmt"
	"github.com/google/uuid"
	"time"
)

type GameAction interface {
	Perform(game *Game) Change
}

type baseAction struct {
	baseEvent
	Created time.Time
}

func (g *Game) getBaseAction(id uuid.UUID, action *Action) baseAction {
	return baseAction{
		baseEvent: baseEvent{
			id:     id,
			action: action,
		},
		Created: time.Now(),
	}
}

func (b *baseAction) ActionCode(g *Game) string {
	entity := g.Entities[b.EntityID()]
	if entity == nil {
		return ""
	}
	return fmt.Sprintf("%T:%s", b, entity.ID.String())
}

type MoveAction struct {
	baseAction
	Position Coordinate
}

func (m *MoveAction) Perform(game *Game) Change {
	entity := game.Entities[m.EntityID()]
	if entity == nil {
		return nil
	}

	pos := m.Position
	entity.Set(pos)
	// Inform the client that the entity moved.
	change := &MoveChange{
		Position:  &pos,
		baseEvent: m.baseEvent,
	}
	return change
}

type RemoveAction struct {
	baseAction
}

func (r *RemoveAction) Perform(game *Game) Change {
	delete(game.Entities, r.EntityID())
	change := &RemoveEntityChange{
		baseEvent: r.baseEvent,
	}
	return change
}

type AddAction struct {
	baseAction
	*Entity
	RemoveOnDisconnect bool
}

func (a *AddAction) Perform(game *Game) Change {
	game.Entities[a.EntityID()] = a.Entity
	if a.RemoveOnDisconnect {
		game.ownedEntities[a.UserID()] = append(game.ownedEntities[a.UserID()], a.EntityID())
	}
	change := &AddEntityChange{
		baseEvent: a.baseEvent,
		Entity:    a.Entity,
	}
	return change
}

type UpdateAction struct {
	baseAction
	*Entity
}

func (u *UpdateAction) Perform(game *Game) Change {
	game.Entities[u.EntityID()] = u.Entity
	change := &AddEntityChange{
		baseEvent: u.baseEvent,
		Entity:    u.Entity,
	}
	return change
}
