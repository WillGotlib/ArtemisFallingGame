using System;

namespace Online
{
    public sealed class Address
    {
        private const string DefaultProtocol = "http";
        private const string DefaultAddr = "localhost"; //"ziv.shalit.name";
        private const int DefaultPort = 37892;

        /// <summary>
        /// Changes the address for the server and connect to it
        /// </summary>
        /// <param name="address">a string url with port (example: localhost:37892)</param>
        /// <returns>Grpc.Core.Channel object</returns>
        public static void ChangeAddress(string address)
        {
            var u = new UriBuilder($"tcp://{address}");
            if (u.Port == -1)
            {
                u.Port = DefaultPort;
            }

            u.Scheme = DefaultProtocol;

            if (u.Host == "")
            {
                u.Host = DefaultAddr;
            }

            _address = u.Uri;
            Connection.Disconnect();
        }

        /// <summary>
        ///  reads the saved address, will not put in the port if its already the default one
        /// </summary>
        /// <returns>address of server</returns>
        public static string GetAddress(bool alwaysIncludePort = false)
        {
            if (alwaysIncludePort || _address.Port != DefaultPort) return _address.Host + ":" + _address.Port;
            return _address.Host;
        }

        public static Uri GetUri(string path="")
        {
            return new Uri(_address, path);
        }

        private static Uri _address = new($"{DefaultProtocol}://{DefaultAddr}:{DefaultPort}");
        
        /*private Address()
        {
        }

        private static Address _instance;

        private static Address connection()
        {
            _instance ??= new Address();

            return _instance;
        }*/
    }
}