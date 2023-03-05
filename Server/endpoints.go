package main

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"net/http"

	"github.com/labstack/echo/v4"
	"google.golang.org/protobuf/proto"
)

func List(e echo.Context) error {
	servers := make([]*pb.Server, 0, len(server.games))
	for u := range server.games {
		servers = append(servers, &pb.Server{
			Id:     u,
			Online: uint32(len(server.sessionUsers[u])),
			Max:    uint32(maxClients),
		})
	}
	serverList := &pb.SessionList{
		Servers: servers,
	}
	message, err := proto.Marshal(serverList)
	if err != nil {
		return err
	}
	return e.Blob(http.StatusOK, "", message)
}

func Connect(e echo.Context) error {
	sessionId := e.Param("session")
	if len(sessionId) < 3 || len(sessionId) > 25 {
		return echo.NewHTTPError(http.StatusBadRequest, "invalid sessionId Provided")
	}
	multiLogger.Info("Incoming connection to", sessionId)

	if server.makeSession(sessionId) {
		go server.removeSession(sessionId, false) // remove after timeout if new session
	}
	sessionUsers := server.sessionUsers[sessionId]
	if len(sessionUsers) >= maxClients {
		return echo.NewHTTPError(http.StatusUnauthorized, "the session is full")
	}

	game := server.games[sessionId]
	entities := game.GetProtoEntities()

	taken := make([]bool, maxClients)
	for _, p := range sessionUsers {
		taken[p.Index] = true
	}
	var index int
	for index = 0; index < maxClients; index++ {
		if taken[index] {
			continue
		}
		break
	}

	client := backend.NewClient(game, index)
	server.addClient(client)

	resp := &pb.ConnectResponse{
		Token:    client.Id.String(),
		Entities: entities,
		Index:    uint32(index),
	}

	message, err := proto.Marshal(resp)
	if err != nil {
		return err
	}
	return e.Blob(http.StatusOK, "", message)
}

func connectServerEcho(c echo.Context) error {
	client, err := server.getClientFromContext(c)
	if err != nil {
		return err
	}
	if client.Active {
		return echo.NewHTTPError(http.StatusUnauthorized, "token in use")
	}

	ws, err := upgrader.Upgrade(c.Response(), c.Request(), nil)
	if err != nil {
		return err
	}

	err = connectServer(client, ws)
	if err != nil {
		log.WithField("websocket", c.Path()).Debug(err)
		return err
	}

	if cancelTimeout, ok := server.gameTimeouts[client.Session.GameId]; ok {
		cancelTimeout() // don't turn off the session someone is joining
	}

	return nil
}
