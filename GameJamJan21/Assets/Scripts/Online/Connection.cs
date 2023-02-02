using System;
using System.Threading.Tasks;
using Grpc.Core;
using UnityEngine;

namespace Online
{
    public sealed class Connection
    {
        private const string DefaultAddr = "ziv.shalit.name"; //"localhost";
        private const int DefaultPort = 37892;

        /// <summary>
        ///  Connects to the saved address
        /// </summary>
        /// <returns>Grpc.Core.Channel object</returns>
        public static Task<Channel> GetChannel()
        {
            return connection()._getChannel();
        }

        /// <summary>
        /// Changes the address for the server and connect to it
        /// </summary>
        /// <param name="address">a string url with port (example: localhost:37892)</param>
        /// <returns>Grpc.Core.Channel object</returns>
        public static Task<Channel> ChangeAddress(string address)
        {
            var u = new UriBuilder("tcp://" + address);
            if (u.Port == -1)
            {
                u.Port = DefaultPort;
            }

            if (u.Host == "")
            {
                u.Host = DefaultAddr;
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
            return _address.Port == DefaultPort ? _address.Host : GetAddress(true);
        }

        public static string GetAddress(bool _)
        {
            return _address.Host + ":" + _address.Port;
        }

        /// <summary>
        /// Disconnects the channel
        /// </summary>
        public static void Dispose()
        {
            connection()._dispose();
        }

        public static ChannelState GetChannelState()
        {
            return _channel == null ? ChannelState.Shutdown : _channel.State;
        }

        private static Channel _channel;
        private static Uri _address = new("tcp://" + DefaultAddr + ":" + DefaultPort);

        private Connection()
        {
            _channel = null;
        }

        private static Connection _instance;

        private static Connection connection()
        {
            _instance ??= new Connection();

            return _instance;
        }

        private async Task<Channel> _getChannel()
        {
            if (_channel != null && _channel.State != ChannelState.Shutdown)
            {
                return _channel;
            }

            _channel = new Channel(GetAddress(true), ChannelCredentials.Insecure);
            await _channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(2));
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