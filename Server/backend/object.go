package backend

import (
	pb "artemisFallingServer/proto"
	"encoding/hex"
	"fmt"
	"github.com/google/uuid"
	"strings"
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
	u := strings.ReplaceAll(e.ID.String(), "-", "")
	id, _ := hex.DecodeString(u)
	return &pb.Entity{
		Id:       id,
		Data:     e.Data,
		Position: e.CurrentPosition.ToProto(),
		Rotation: e.CurrentRotation.ToProto(),
		Type:     e.Type,
	}
}

func EntityFromProto(entity *pb.Entity) (*Entity, error) {
	u := fmt.Sprintf("%x", entity.GetId())
	id, err := uuid.Parse(u)
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
