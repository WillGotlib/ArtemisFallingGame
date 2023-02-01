using System;
using Grpc.Core;
using UnityEngine;

namespace Online
{
    public sealed class Connection
    {
        private const int DefaultPort = 37892;
        
        /// <summary>
        ///  Connects to the saved address
        /// </summary>
        /// <returns>Grpc.Core.Channel object</returns>
        public static Channel GetChannel()
        {
            return connection()._getChannel();
        }

        /// <summary>
        /// Changes the address for the server and connect to it
        /// </summary>
        /// <param name="address">a string url with port (example: localhost:50051)</param>
        /// <returns>Grpc.Core.Channel object</returns>
        public static Channel ChangeAddress(string address)
        {
            var u  = new UriBuilder("tcp://"+address);
            if (u.Port == -1)
            {
                u.Port = DefaultPort;
            }
            if (u.Host == "")
            {
                u.Host = "localhost";
            }

            if (_address.Equals(u.Uri)) return GetChannel();
            Dispose();
            _address = u.Uri;
            return GetChannel();
        }

        /// <summary>
        ///  reads the saved address
        /// </summary>
        /// <returns>address of server</returns>
        public static string GetAddress()
        {
            return _address.Port == DefaultPort ? _address.Host :GetAddress(true);
        }

        public static string GetAddress(bool _)
        {
            return _address.Host+":"+_address.Port;
        }

        /// <summary>
        /// Disconnects the channel
        /// </summary>
        public static void Dispose()
        {
            connection()._dispose();
        }

        private Channel _channel;
        private static Uri _address = new ("tcp://localhost:"+DefaultPort);

        private Connection()
        {
        }

        private static Connection _instance;

        private static Connection connection()
        {
            _instance ??= new Connection();

            return _instance;
        }

        private Channel _getChannel()
        {
            if (_channel != null && _channel.State != ChannelState.Shutdown)
            {
                return _channel;
            }

            _channel = new Channel(GetAddress(true), ChannelCredentials.Insecure);
            return _channel;
        }

        private async void _dispose()
        {
            if (_channel != null)
            {
                Debug.Log("Shutting down channel");
                await _channel.ShutdownAsync();
            }

            _instance = null;
        }
    }
}