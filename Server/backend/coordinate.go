package backend

import (
	pb "artemisFallingServer/proto"
	"math"
)

type Coordinate struct {
	X float64
	Y float64
	Z float64
}

func (c *Coordinate) Distance(o *Coordinate) float64 {
	dx := o.X - c.X
	dy := o.Y - c.Y
	dz := o.Z - c.Z
	return math.Sqrt(dx*dx + dy*dy + dz + dz)
}

func (c *Coordinate) ToProto() *pb.Position {
	return &pb.Position{
		X: float32(c.X),
		Y: float32(c.Y),
		Z: float32(c.Z),
	}
}

func CoordinateFromProto(position *pb.Position) *Coordinate {
	return &Coordinate{
		X: float64(position.X),
		Y: float64(position.Y),
		Z: float64(position.Z),
	}
}
