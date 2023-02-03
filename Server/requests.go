package main

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"log"
)

func (s *GameServer) handleRequests(request *backend.Action) {
	switch request.Request.GetAction().(type) {
	case *pb.StreamAction_MoveEntity:
		s.handleMoveRequest(request)
	case *pb.StreamAction_AddEntity:
		s.handleAddRequest(request)
	case *pb.StreamAction_RemoveEntity:
		s.handleRemoveRequest(request)
	case *pb.StreamAction_UpdateEntity:
		s.handleUpdateRequest(request)
	default:
		log.Println("unknown request", request)
	}
}

func (s *GameServer) handleMoveRequest(req *backend.Action) {
	request := req.Request.GetMoveEntity()
	id, ok := ParseID(request.GetId())
	if !ok {
		log.Println("can't parse id to move")
		return
	}
	req.Game().MoveEntity(id, req,
		backend.CoordinateFromProto(request.GetPosition()), backend.RotationFromProto(request.GetRotation()))
}

func (s *GameServer) handleRemoveRequest(req *backend.Action) {
	request := req.Request.GetRemoveEntity()
	id, ok := ParseID(request.GetId())
	if !ok {
		log.Println("can't parse id to remove")
		return
	}
	req.Game().RemoveEntity(id, req)
}

func (s *GameServer) handleAddRequest(req *backend.Action) {
	request := req.Request.GetAddEntity()
	ent, err := backend.EntityFromProto(request.GetEntity())
	if err != nil {
		log.Println("can't parse entity to add")
		return
	}

	req.Game().AddEntity(ent, req, !request.GetKeepOnDisconnect())
}

func (s *GameServer) handleUpdateRequest(req *backend.Action) {
	request := req.Request.GetUpdateEntity()
	ent, err := backend.EntityFromProto(request.GetEntity())
	if err != nil {
		log.Println("can't parse entity to update")
		return
	}

	req.Game().UpdateEntity(ent, req)
}
