using Google.Protobuf;
using Google.Protobuf.Collections;
using protoBuff;
using Proyecto26;
using RSG;
using Unity.Services.Core;
using Unity.WebRTC;

namespace Online
{
    public sealed class Connection //todo rate limit
    {
        private const string GameServerProof = "artemis-falling-server";

        public static Promise<bool> IsGameServer()
        {
            var returnP = new Promise<bool>();
            RestClient.Get(new RequestHelper
            {
                Uri = Address.GetUri().ToString(),
                Timeout = 2,
                Retries = 0,
                DefaultContentType = false,
                ParseResponseBody = false
            }).Then(r =>
                returnP.Resolve(r.StatusCode == 204 && r.GetHeader("Game") == GameServerProof)
            ).Catch(returnP.Reject);
            return returnP;
        }

        /////////
        public static void SendPriority(Request request)
        {
            var c = GetConnection();
            if (c.RtcAlive())
                c._rtcConnection.SendPriority(request);
        }

        public static void SendFast(Request request)
        {
            var c = GetConnection();
            if (c.RtcAlive())
                c._rtcConnection.SendFast(request);
        }

        public static void SendPriority(StreamAction request)
        {
            SendPriority(new Request { Requests = { request } });
        }

        public static void SendFast(StreamAction request)
        {
            SendFast(new Request { Requests = { request } });
        }

        //////////

        /// <summary>
        /// Sends a connect request to the server
        /// </summary>
        /// <param name="session">The session id/name can be anything, new names will create a new session</param>
        /// <returns>list of protoBuff.Entity's present in the session</returns>
        public static Promise<RepeatedField<Entity>> Connect(string session)
        {
            return GetConnection()._connect(session);
        }

        /// <summary>
        /// Lists all the active sessions in on the server
        /// </summary>
        /// <returns>A list of protoBuff.Server objects</returns>
        public static Promise<RepeatedField<Server>> List()
        {
            return GetConnection()._list();
        }

        /// <summary>
        /// Starts the stream to the server
        /// </summary>
        public static Promise StartStream()
        {
            var conn = GetConnection();
            if (conn.RtcAlive() || conn._token == "") return null; // maybe will cause issues

            conn._rtcConnection = new WebRtc { callback = _callback, onClose = _onClose };
            return conn._rtcConnection.Connect(conn._token);
        }

        /// <summary>
        /// Turns off the stream and disconnects from the channel
        /// </summary>
        public static void Disconnect()
        {
            var conn = GetConnection();
            if (!conn.RtcAlive()) return;

            conn._rtcConnection.Disconnect();
            conn._rtcConnection = null;
            _instance = null;
        }


        /// <summary>
        /// Registers a callback that will be used when a message is received from the server
        /// </summary>
        /// <param name="callback">A function that takes in a protoBuff.Response and returns nothing</param>
        public static void RegisterMessageCallback(OnMessageCallback callback)
        {
            _callback = callback;
            if (GetConnection()._rtcConnection != null)
                GetConnection()._rtcConnection.callback = callback;
        }

        public static int GetIndex()
        {
            return GetConnection()._index;
        }

        public static bool IsStreaming()
        {
            return GetConnection().RtcAlive();
        }


        public static void OnClose(DelegateOnClose onClose)
        {
            _onClose = () =>
            {
                Disconnect();
                onClose();
            };
            if (GetConnection().RtcAlive()) GetConnection()._rtcConnection.onClose = _onClose;
        }

        private int _index;
        private string _token;
        private static OnMessageCallback _callback;
        private static DelegateOnClose _onClose;

        private WebRtc _rtcConnection;

        private bool RtcAlive()
        {
            return _rtcConnection != null && _rtcConnection.Alive;
        }

        private Connection()
        {
            _index = -1;
        }

        private static Connection _instance;

        private static Connection GetConnection()
        {
            _instance ??= new Connection();
            return _instance;
        }

        private Promise<RepeatedField<Entity>> _connect(string session)
        {
            var returnP = new Promise<RepeatedField<Entity>>();
            RestClient.Post(
                    new RequestHelper
                    {
                        Uri = Address.GetUri($"/connect/{session}").ToString(),
                        Retries = 0,
                        DefaultContentType = false,
                        ParseResponseBody = false,
                    })
                .Then(r =>
                {
                    if (r.StatusCode != 200)
                    {
                        returnP.Reject(new RequestFailedException((int)r.StatusCode, "Failed to request connection"));
                        return;
                    }

                    var conn = ConnectResponse.Parser.ParseFrom(r.Data);
                    _token = conn.Token;
                    _index = (int)conn.Index;
                    returnP.Resolve(conn.Entities);
                })
                .Catch(returnP.Reject);
            return returnP;
        }

        private Promise<RepeatedField<Server>> _list()
        {
            var returnP = new Promise<RepeatedField<Server>>();
            RestClient.Get(new RequestHelper
                {
                    Uri = Address.GetUri($"/list").ToString(),
                    Timeout = 5,
                    Retries = 0,
                    DefaultContentType = false,
                    ParseResponseBody = false
                })
                .Then(r =>
                {
                    if (r.Data == null)
                    {
                        returnP.Resolve(new RepeatedField<Server>());
                        return;
                    }

                    var sessions = SessionList.Parser.ParseFrom(r.Data);
                    returnP.Resolve(sessions.Servers);
                })
                .Catch(returnP.Reject);
            return returnP;
        }
    }
}