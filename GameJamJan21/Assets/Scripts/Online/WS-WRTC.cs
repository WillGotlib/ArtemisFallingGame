using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using protoBuff;
using Unity.WebRTC;
using NativeWebSocket;
using RSG;
using UnityEngine;

namespace Online
{
    internal class WS_WRTC
    {
        public Promise Connect(MonoBehaviour parent, string token)
        {
            _promise = new Promise();

            var addr = new UriBuilder(Address.GetUri("/stream"));
            addr.Scheme = "ws";
            _webSocket = new WebSocket(addr.Uri.ToString(),
                new Dictionary<string, string> { { "Token", token } });

            _webSocket.OnError += e =>
            {
                Debug.Log("ws Error! " + e);
                _promise.Reject(new Exception(e));
            };
            _webSocket.OnClose += e =>
            {
                Debug.Log("ws Connection closed! " + e); 
                if (e != WebSocketCloseCode.Normal) _promise.Reject(new Exception("close "+e));
            };
            _webSocket.OnMessage += WsICE;
            _webSocket.OnOpen += () => parent.StartCoroutine(NegotiatePeer(parent));

            _webSocket.Connect();
            return _promise;
        }

        private WebSocket _webSocket;
        private RTCPeerConnection _rtcPeer;
        private Promise _promise;

        public WS_WRTC(RTCPeerConnection peer)
        {
            _rtcPeer = peer;
            _rtcPeer.OnIceCandidate = OnIceCandidate;
            _rtcPeer.OnIceConnectionChange = OnIceConnectionChange;
        }

        private void WsICE(byte[] bytes)
        {
            var incoming = Trickle.Parser.ParseFrom(bytes);

            switch (incoming.MessageCase)
            {
                case Trickle.MessageOneofCase.Candidate:
                    var candidate = new RTCIceCandidateInit
                    {
                        candidate = incoming.Candidate.Candidate,
                        sdpMid = incoming.Candidate.SDPMid,
                    };
                    if (incoming.Candidate.HasSDPMLineIndex)
                    {
                        candidate.sdpMLineIndex = incoming.Candidate.SDPMLineIndex;
                    }

                    _rtcPeer.AddIceCandidate(new RTCIceCandidate(candidate));
                    break;
                case Trickle.MessageOneofCase.Description:
                    var a = new RTCSessionDescription
                    {
                        sdp = incoming.Description.SDP,
                        type = (RTCSdpType)incoming.Description.Type,
                    };
                    _rtcPeer.SetRemoteDescription(ref a);
                    break;
            }
        }

        public void OnIceCandidate(RTCIceCandidate ice)
        {
            if (ice.Candidate != "")
            {
                var cand = new ICECandidateInit
                {
                    Candidate = ice.Candidate,
                    SDPMid = ice.SdpMid,
                    UsernameFragment = ice.UserNameFragment
                };
                if (ice.SdpMLineIndex != null)
                {
                    cand.SDPMLineIndex = (int)ice.SdpMLineIndex;
                }

                _webSocket.Send(new Trickle { Candidate = cand }.ToByteArray());
            }
        }

        private void OnIceConnectionChange(RTCIceConnectionState state)
        {
            Debug.Log("ice state " + state);

            if (state == RTCIceConnectionState.Connected || state == RTCIceConnectionState.Completed)
            {
                Debug.Log("connected, closing websocket");
                _webSocket.Close();
                _webSocket = null;
                _promise.Resolve();
            }
        }

        private IEnumerator NegotiatePeer(MonoBehaviour parent)
        {
            var op = _rtcPeer.CreateOffer();
            yield return op;

            if (!op.IsError)
            {
                // Debug.Log("creating offer");
                yield return parent.StartCoroutine(OnCreateOfferSuccess(op.Desc));
            }
            else
            {
                Debug.LogError($"Error Detail Type: {op.Error.message}");
            }
        }

        private IEnumerator OnCreateOfferSuccess(RTCSessionDescription desc)
        {
            var op = _rtcPeer.SetLocalDescription(ref desc);
            yield return op;

            if (op.IsError)
            {
                Debug.LogError(op.Error);
                _webSocket.Close();
                _rtcPeer.Close();
                _promise.Reject(new Exception(op.Error.message));
                yield break;
            }

            // Debug.Log($"sending offer to remote\n{desc}");
            _webSocket.Send(new Trickle
            {
                Description = new SessionDescription
                {
                    SDP = desc.sdp,
                    Type = (int)desc.type
                }
            }.ToByteArray());
        }
    }
}