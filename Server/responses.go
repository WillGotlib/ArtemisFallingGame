package main

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"log"
)

func (s *GameServer) watchChanges() {
	go func() {
		for {
			change := <-s.ChangeChannel
			switch c := change.(type) {
			case *backend.UpdateEntityChange:
				s.handleUpdateChange(*c)
			case *backend.RemoveEntityChange:
				s.handleRemoveChange(*c)
			case *backend.AddEntityChange:
				s.handleAddChange(*c)
			case *backend.MoveChange:
				s.handleMoveChange(*c)
			default:
				log.Println("unknown type", c)
			}
		}
	}()
}

func (s *GameServer) handleUpdateChange(change backend.UpdateEntityChange) {
	resp := &pb.StreamAction{
		Action: &pb.StreamAction_UpdateEntity{
			UpdateEntity: &pb.UpdateEntity{
				Entity: change.Entity.ToProto(),
			},
		},
	}
	s.broadcast(change.Action().AddResponse(resp))
}

func (s *GameServer) handleMoveChange(change backend.MoveChange) {
	resp := &pb.StreamAction{
		Action: &pb.StreamAction_MoveEntity{
			MoveEntity: &pb.MoveEntity{
				Position: change.Position.ToProto(),
				Id:       change.EntityID().String(),
			},
		},
	}

	s.broadcast(change.Action().AddResponse(resp))
}

func (s *GameServer) handleRemoveChange(change backend.RemoveEntityChange) {
	resp := &pb.StreamAction{
		Action: &pb.StreamAction_RemoveEntity{
			RemoveEntity: &pb.RemoveEntity{
				Id: change.EntityID().String(),
			},
		},
	}
	s.broadcast(change.Action().AddResponse(resp))
}

func (s *GameServer) handleAddChange(change backend.AddEntityChange) {
	resp := &pb.StreamAction{
		Action: &pb.StreamAction_AddEntity{
			AddEntity: &pb.AddEntity{
				Entity: change.Entity.ToProto(),
			},
		},
	}
	s.broadcast(change.Action().AddResponse(resp))
}
