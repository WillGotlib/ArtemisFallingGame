using System;
using System.Collections;
using Google.Protobuf;
using NativeWebSocket;
using protoBuff;
using Unity.WebRTC;
using UnityEngine;

public class WSTest : MonoBehaviour
{
    WebSocket websocket;
    private RTCPeerConnection pc;
    private RTCDataChannel dataChannel;

    async void Awake()
    {
        websocket = new WebSocket("ws://localhost:37892/stream");

        websocket.OnError += e => { Debug.Log("ws Error! " + e); };
        websocket.OnClose += e => { Debug.Log("ws Connection closed! " + e); };
        websocket.OnMessage += bytes =>
        {
            Debug.Log("OnMessage!");

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

                    pc.AddIceCandidate(new RTCIceCandidate(candidate));
                    break;
                case Trickle.MessageOneofCase.Description:
                    var a = new RTCSessionDescription
                    {
                        sdp = incoming.Description.SDP,
                        type = (RTCSdpType)incoming.Description.Type,
                    };
                    pc.SetRemoteDescription(ref a);
                    break;
            }
        };

        websocket.OnOpen += Call;

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private void OnApplicationQuit()
    {
        Hangup();
    }

    private void Call()
    {
        var config = new RTCConfiguration
        {
            iceServers = new []{ new RTCIceServer{urls = new []{"stun:stun.l.google.com:19302"}}}
        };
        pc = new RTCPeerConnection(ref config);
        pc.OnIceCandidate = OnIceCandidate;
        pc.OnIceConnectionChange = OnIceConnectionChange;
        
        var conf = new RTCDataChannelInit();
        dataChannel = pc.CreateDataChannel("priority", conf);
        dataChannel.OnOpen = () =>
        {
            Debug.Log("stable channel open");
        };
        dataChannel.OnMessage = e => { Debug.Log("dc: " + System.Text.Encoding.UTF8.GetString(e)); };
        
        var fastConf = new RTCDataChannelInit
        {
            maxRetransmits = 0,
            ordered = false,
        };
        dataChannel = pc.CreateDataChannel("fast", fastConf);
        dataChannel.OnOpen = () =>
        {
            Debug.Log("fast channel open");
            dataChannel.Send("thing");
        };
        dataChannel.OnMessage = e => { Debug.Log("dc: " + System.Text.Encoding.UTF8.GetString(e)); };
        
        
        Debug.Log("everything ready to call");
        StartCoroutine(PeerNegotiation());
    }

    private async void Hangup()
    {
        if (pc != null)
        {
            pc.Close();
            pc.Dispose();
            pc = null;
        }

        if (websocket != null)
        {
            await websocket.Close();
            websocket = null;
        }
    }

    private void OnIceCandidate(RTCIceCandidate ice)
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

            websocket.Send(new Trickle { Candidate = cand }.ToByteArray());
        }
    }

    private void OnIceConnectionChange(RTCIceConnectionState state)
    {
        Debug.Log("ice state " + state);

        if (state == RTCIceConnectionState.Connected || state == RTCIceConnectionState.Completed)
        {
            Debug.Log("connected, closing websocket");
        }
    }

    IEnumerator PeerNegotiation()
    {
        var op = pc.CreateOffer();
        yield return op;

        if (!op.IsError)
        {
            Debug.Log("creating offer");
            yield return StartCoroutine(OnCreateOfferSuccess(op.Desc));
        }
        else
        {
            Debug.LogError($"Error Detail Type: {op.Error.message}");
        }
    }

    private IEnumerator OnCreateOfferSuccess(RTCSessionDescription desc)
    {
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (op.IsError)
        {
            Debug.LogError(op.Error);
            Hangup();
            yield break;
        }

        Debug.Log($"sending offer to remote\n{desc}");
        websocket.Send(new Trickle
        {
            Description = new SessionDescription
            {
                SDP = desc.sdp,
                Type = (int)desc.type
            }
        }.ToByteArray());
    }
}