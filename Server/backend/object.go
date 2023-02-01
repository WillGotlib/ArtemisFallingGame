package backend

import (
	pb "artemisFallingServer/proto"
	"github.com/google/uuid"
)

type Entity struct {
	ID              uuid.UUID
	CurrentPosition *Coordinate
	CurrentRotation *Rotation
	Data            string
	Type            string
}

// Set sets the position of the player.
func (e *Entity) Set(c *Coordinate, r *Rotation) {
	e.CurrentPosition = c
	e.CurrentRotation = r
}

func (e *Entity) ToProto() *pb.Entity {
	return &pb.Entity{
		Id:       e.ID.String(),
		Data:     e.Data,
		Position: e.CurrentPosition.ToProto(),
		Rotation: e.CurrentRotation.ToProto(),
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
		CurrentRotation: RotationFromProto(entity.GetRotation()),
		Data:            entity.Data,
		Type:            entity.GetType(),
	}, nil
}
