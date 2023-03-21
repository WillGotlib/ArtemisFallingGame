using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using protoBuff;
using Unity.WebRTC;
using Proyecto26;
using RSG;
using Unity.Services.Core;
using UnityEngine;

namespace Online
{
    internal class WRTCConnector : MonoBehaviour
    {
        public Promise Connect(string token, RTCPeerConnection peer)
        {
            _promise = new Promise();
            _token = token;

            SetPeer(peer);
            
            return _promise;
        }

        private RTCPeerConnection _rtcPeer;
        private Promise _promise;
        private string _token;

        private void SetPeer(RTCPeerConnection peer)
        {
            _rtcPeer = peer;
            _rtcPeer.OnIceCandidate = OnIceCandidate;
            _rtcPeer.OnIceConnectionChange = OnIceConnectionChange;
            _rtcPeer.OnNegotiationNeeded = ()=> StartCoroutine(NegotiatePeer());
        }

        private void OnIceCandidate(RTCIceCandidate ice)
        {
            Debug.Log($"{ice} {ice.Type}");
            if (ice.Type != RTCIceCandidateType.Srflx)
                return;
            
            SendRequest(_rtcPeer.LocalDescription);
        }

        private void OnIceConnectionChange(RTCIceConnectionState state)
        {
            Debug.Log("ice state " + state);

            if (state == RTCIceConnectionState.Completed)
            {
                Debug.Log("connected");
                //_webSocket.Close();
                _promise.Resolve();
            }
        }

        private IEnumerator NegotiatePeer()
        {
            var op = _rtcPeer.CreateOffer();
            yield return op;

            if (op.IsError)
            {
                Debug.LogError($"Error Detail Type: {op.Error.message}");
                yield break;
            }

            yield return StartCoroutine(OnCreateOfferSuccess(op.Desc));
        }

        private IEnumerator OnCreateOfferSuccess(RTCSessionDescription desc)
        {
            var op = _rtcPeer.SetLocalDescription(ref desc);
            yield return op;

            if (op.IsError)
            {
                Debug.LogError(op.Error);
                _rtcPeer.Close();
                _promise.Reject(new Exception(op.Error.message));
            }
        }

        private void SendRequest(RTCSessionDescription desc)
        {
            var body = new SessionDescription
            {
                SDP = desc.sdp,
                Type = (int)desc.type
            };
            
            RestClient.Post(
                    new RequestHelper
                    {
                        Uri = Address.GetUri("/stream").ToString(),
                        Retries = 0,
                        DefaultContentType = false,
                        ParseResponseBody = false,
                        Headers = new Dictionary<string, string> { { "Authorization", _token } },
                        BodyRaw = body.ToByteArray()
                    })
                .Then(r =>
                {
                    if (r.StatusCode != 200)
                    {
                        _promise.Reject(new RequestFailedException((int)r.StatusCode, "Failed to start stream"));
                        return;
                    }

                    var answer = SessionDescription.Parser.ParseFrom(r.Data);
                    var a = new RTCSessionDescription
                    {
                        sdp = answer.SDP,
                        type = (RTCSdpType)answer.Type,
                    };
                    _rtcPeer.SetRemoteDescription(ref a);
                }).Catch(_promise.Reject);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}