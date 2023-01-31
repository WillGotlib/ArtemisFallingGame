package backend

import (
	pb "artemisFallingServer/proto"
	"github.com/google/uuid"
)

type Entity struct {
	ID              uuid.UUID
	CurrentPosition Coordinate
	Data            string
	Type            string
}

// Position determines the player position.
func (e *Entity) Position() Coordinate {
	return e.CurrentPosition
}

// Set sets the position of the player.
func (e *Entity) Set(c Coordinate) {
	e.CurrentPosition = c
}

func (e *Entity) ToProto() *pb.Entity {
	return &pb.Entity{
		Id:       e.ID.String(),
		Data:     e.Data,
		Position: e.Position().ToProto(),
		Type:     e.Type,
	}
}

func EntityFromProto(entity *pb.Entity) (*Entity, error) {
	id, err := uuid.Parse(entity.GetId())
	if err != nil {
		return nil, err
	}
	return &Entity{
		ID:              id,
		CurrentPosition: CoordinateFromProto(entity.GetPosition()),
		Data:            entity.Data,
		Type:            entity.GetType(),
	}, nil
}
