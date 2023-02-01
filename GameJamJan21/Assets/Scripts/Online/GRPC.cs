using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using protoBuff;
using UnityEngine;
using Channel = System.Threading.Channels.Channel;
using Server = protoBuff.Server;
using StatusCode = Grpc.Core.StatusCode;

namespace Online
{
    public delegate void OnMessageCallback(Response action);

    public sealed class GRPC
    {
        /// <summary>
        /// Sends a connect request to the server
        /// </summary>
        /// <param name="session">The session id/name can be anything, new names will create a new session</param>
        /// <returns>list of protoBuff.Entity's present in the session</returns>
        public static RepeatedField<Entity> Connect(string session)
        {
            return Grpc()._connect(session);
        }

        /// <summary>
        /// Lists all the active sessions in on the server
        /// </summary>
        /// <returns>A list of protoBuff.Server objects</returns>
        public static RepeatedField<Server> List()
        {
            return Grpc()._client.List(new SessionRequest()).Servers;
        }

        /// <summary>
        /// Starts the stream to the server
        /// </summary>
        public static void StartStream()
        {
            Grpc()._startStream();
        }

        /// <summary>
        /// Turns off the stream and disconnects from the channel
        /// </summary>
        public static void Disconnect()
        {
            Grpc()._disconnect();
        }

        /// <summary>
        /// Registers a callback that will be used when a message is received from the server
        /// </summary>
        /// <param name="callback">A function that takes in a protoBuff.Response and returns nothing</param>
        public static void RegisterMessageCallback(OnMessageCallback callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Send a collection of requests over the GRPC channel, unsent request will be put in a queue that gets sent once there is a connection
        /// </summary>
        /// <param name="request">A protoBuff.Request to send over the channel</param>
        public static async void SendRequest(Request request)
        {
            if (_active)
                await Grpc()._queue.Writer.WriteAsync(request);
            else
                IdleQueue.Enqueue(request);
        }

        /// <summary>
        /// Send a single request over GRPC
        /// </summary>
        /// <param name="request">A protoBuff.StreamAction to send</param>
        public static void SendRequest(StreamAction request)
        {
            SendRequest(new Request { Requests = { request } });
        }

        public static Exception error;
        private readonly Game.GameClient _client;
        private AsyncDuplexStreamingCall<Request, Response> _stream;
        private string _token;
        private static bool _active;
        private static OnMessageCallback _callback = Debug.Log; // just log the responses by default
        private readonly Channel<Request, Request> _queue;
        private static readonly Queue<Request> IdleQueue = new();

        private GRPC()
        {
            _active = false;
            _queue = Channel.CreateUnbounded<Request>();
            _client = new Game.GameClient(Connection.GetChannel());
        }

        private static GRPC _instance;

        private static GRPC Grpc()
        {
            _instance ??= new GRPC();
            return _instance;
        }

        private RepeatedField<Entity> _connect(string session)
        {
            var conn = _client.Connect(new ConnectRequest { Session = session });
            _token = conn.Token;
            return conn.Entities;
        }

        private void _startStream()
        {
            if (_client == null)
            {
                throw new Exception("No connection");
            }

            _stream = _client.Stream(new Metadata
            {
                new("authorization", _token)
            });
            _active = true;
            Task.Run(_readStreamData);
            Task.Run(_messageWriter);

            while (IdleQueue.Count > 0)
                SendRequest(IdleQueue.Dequeue());
        }

        private async void _disconnect()
        {
            if (!_active) return;
            _active = false;
            await _queue.Writer.WriteAsync(new Request());
            Debug.Log("Shutting down stream");
            Connection.Dispose();
            _instance = null;
        }

        private async void _readStreamData()
        {
            try
            {
                while (await _stream.ResponseStream.MoveNext())
                {
                    var action = _stream.ResponseStream.Current;
                    _callback(action);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Debug.Log("Stream cancelled");
            }
        }

        private async void _messageWriter()
        {
            while (true)
            {
                var req = await _queue.Reader.ReadAsync();
                if (!_active)
                {
                    await _stream.RequestStream.CompleteAsync();
                    return;
                }

                try
                {
                    await _stream.RequestStream.WriteAsync(req);
                }
                catch (RpcException e)
                {
                    error = e;
                }
            }
        }
    }
}