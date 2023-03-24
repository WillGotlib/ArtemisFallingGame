package main

import (
	"artemisFallingServer/backend"
	pb "artemisFallingServer/proto"
	"errors"
	"github.com/labstack/echo/v4"
	"github.com/pion/webrtc/v3"
	"github.com/sirupsen/logrus"
	"github.com/snaka/whatsmyip/lib/whatsmyip"
	"google.golang.org/protobuf/proto"
	"io"
	"net/http"
)

var (
	IceConfig webrtc.Configuration
	webrtcApi *webrtc.API

	webRTClog *logrus.Entry
)

func init() {
	webRTClog = log.WithField("mode", "webrtc")

	IceConfig = webrtc.Configuration{
		ICEServers: []webrtc.ICEServer{
			{
				URLs: []string{"stun:stun.l.google.com:19302"},
			},
		},
	}
}

func SetupWRTCEngine() {
	publicIp, err := whatsmyip.DiscoverPublicIPSync()
	if err != nil {
		log.WithError(err).Error("cant get public ip")
		return
	}
	log.Infof("Public ip is '%s'", publicIp)

	s := webrtc.SettingEngine{}
	s.SetNAT1To1IPs([]string{publicIp}, webrtc.ICECandidateTypeHost)
	err = s.SetEphemeralUDPPortRange(52000, 52100)
	if err != nil {
		webRTClog.WithError(err).Error("cant set port range limit")
		return
	}

	webrtcApi = webrtc.NewAPI(webrtc.WithSettingEngine(s))
}

func connectServer(client *backend.Client, c echo.Context) error {
	clientLog := webRTClog.WithField("client", client.Id)

	body, err := io.ReadAll(c.Request().Body)
	if err != nil || len(body) == 0 {
		return errors.New("error reading body")
	}

	peerConnection, err := webrtcApi.NewPeerConnection(IceConfig)
	if err != nil {
		clientLog.WithError(err).Error("cant make new peer connection")
		return err
	}
	client.WRTC = peerConnection

	peerConnection.OnConnectionStateChange(func(state webrtc.PeerConnectionState) {
		clientLog.Debug("state is now ", state)

		if state == webrtc.PeerConnectionStateConnected {
			clientLog.Debug("client now active")
			client.Active = true
		}

		if state == webrtc.PeerConnectionStateFailed || state == webrtc.PeerConnectionStateDisconnected || state == webrtc.PeerConnectionStateClosed { // remove even a temp disconnected client
			server.removeClient(client.Id, "Disconnected")
		}
	})

	peerConnection.OnDataChannel(func(d *webrtc.DataChannel) {
		d.OnOpen(func() {
			clientLog.Debugf("got datachannel: %s", d.Label())
			switch d.Label() {
			case "priority":
				client.PriorityChannel = d
			case "fast":
				client.FastChannel = d
			default:
				clientLog.Info("unknown datachannel type", d.Label())
			}
		})

		d.OnMessage(server.parseMessage(d.Label(), client))
	})

	var desc = new(pb.SessionDescription)
	err = proto.Unmarshal(body, desc)
	if err != nil {
		return err
	}
	offer := webrtc.SessionDescription{
		Type: webrtc.SDPType(desc.GetType() + 1),
		SDP:  desc.GetSDP(),
	}

	if err = peerConnection.SetRemoteDescription(offer); err != nil {
		return err
	}

	answer, err := peerConnection.CreateAnswer(nil)
	if err != nil {
		return err
	}

	gatherComplete := webrtc.GatheringCompletePromise(peerConnection)

	if err = peerConnection.SetLocalDescription(answer); err != nil {
		return err
	}

	// block ice -- send all at once
	<-gatherComplete

	description := peerConnection.LocalDescription()
	resp := &pb.SessionDescription{
		Type: int32(description.Type) - 1,
		SDP:  description.SDP,
	}
	d, err := proto.Marshal(resp)
	if err != nil {
		clientLog.WithError(err).Error("Error while marshaling session description")
		return err
	}

	return c.Blob(http.StatusOK, "", d)
}
