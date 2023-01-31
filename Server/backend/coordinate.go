package backend

import (
	pb "artemisFallingServer/proto"
	"math"
)

type Coordinate struct {
	X float64
	Y float64
}

func (c Coordinate) Distance(o Coordinate) float64 {
	return math.Sqrt(math.Pow(o.X-c.X, 2) + math.Pow(o.Y-c.Y, 2))
}

func (c Coordinate) ToProto() *pb.Position {
	return &pb.Position{
		X: float32(c.X),
		Y: float32(c.Y),
	}
}

func CoordinateFromProto(position *pb.Position) Coordinate {
	return Coordinate{
		X: float64(position.X),
		Y: float64(position.Y),
	}
}
