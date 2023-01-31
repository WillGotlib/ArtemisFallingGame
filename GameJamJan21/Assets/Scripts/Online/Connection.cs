using Grpc.Core;
using UnityEngine;

namespace Online
{
    public sealed class Connection
    {
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
            Dispose();
            _address = address;
            return GetChannel();
        }

        /// <summary>
        ///  reads the saved address
        /// </summary>
        /// <returns>address of server</returns>
        public static string GetAddress()
        {
            return _address;
        }

        /// <summary>
        /// Disconnects the channel
        /// </summary>
        public static void Dispose()
        {
            connection()._dispose();
        }

        private Channel _channel;
        private static string _address = "localhost:50051";

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

            _channel = new Channel(_address, ChannelCredentials.Insecure);
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