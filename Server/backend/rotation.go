package backend

import (
	pb "artemisFallingServer/proto"
)

type Rotation struct {
	X float64
	Y float64
	Z float64
	W float64
}

func (q *Rotation) ToProto() *pb.Rotation {
	return &pb.Rotation{
		X: float32(q.X),
		Y: float32(q.Y),
		Z: float32(q.Z),
		W: float32(q.W),
	}
}

func RotationFromProto(position *pb.Rotation) *Rotation {
	return &Rotation{
		X: float64(position.X),
		Y: float64(position.Y),
		Z: float64(position.Z),
		W: float64(position.W),
	}
}
