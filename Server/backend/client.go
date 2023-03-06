package backend

import (
	"time"

	"github.com/pion/webrtc/v3"
	log "github.com/sirupsen/logrus"
)

type Client struct {
	WRTC            *webrtc.PeerConnection
	PriorityChannel *webrtc.DataChannel
	FastChannel     *webrtc.DataChannel
	Active          bool
	LastMessage     time.Time
	Id              Token
	Session         *Game
	Index           int
}

func NewClient(game *Game, index int) *Client {
	return &Client{
		Id:          NewToken(),
		LastMessage: time.Now(),
		Session:     game,
		Index:       index,
		Active:      false,
	}
}

func NilClient(client *Client) *Client {
	return &Client{
		Session: client.Session,
	}
}

// Done closes and kills the clients connection
func (c *Client) Done(message string) {
	if c == nil || c.WRTC == nil || c.WRTC.SignalingState() == webrtc.SignalingStateClosed {
		return
	}
	if c.PriorityChannel != nil && c.PriorityChannel.ReadyState() == webrtc.DataChannelStateOpen {
		err := c.PriorityChannel.SendText(message)
		if err != nil {
			log.Debug(err)
		}
		err = c.PriorityChannel.Close()
		if err != nil {
			log.Error(err)
		}
		err = c.FastChannel.Close()
		if err != nil {
			log.Error(err)
		}
		c.PriorityChannel = nil
		c.FastChannel = nil
	}

	if err := c.WRTC.Close(); err != nil {
		log.Debug("failed to close peer connection")
	}
	c.WRTC = nil
}
