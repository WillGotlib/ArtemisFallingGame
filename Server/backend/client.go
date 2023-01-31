package backend

import (
	pb "artemisFallingServer/proto"
	"time"
)

type Client struct {
	StreamServer pb.Game_StreamServer
	LastMessage  time.Time
	Done         chan error
	Id           Token
	Session      *Game
}

func NewClient(game *Game) *Client {
	return &Client{
		Id:          NewToken(),
		Done:        make(chan error, 1),
		LastMessage: time.Now(),
		Session:     game,
	}
}

func NilClient(client *Client) *Client {
	return &Client{
		Session: client.Session,
	}
}
