using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;
using UnityEngine;
using Event = Analytics.AnalyticsEvent.Types.Event;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private string _loggingVersion = "0.0.1";
        private byte[] _magic = { 0x89, 0x41, 0x52, 0x54 }; // identifiable byte sequence 

        [SerializeField] private int loggingFPS = 30;
        [SerializeField] private string gameVersion;

        [SerializeField] private string logPath = "GameLogs/";

        private FileStream _loggingStream;
        private BinaryWriter _loggingFile;

        private readonly Dictionary<string, int> _idMap = new();
        private int _currId = 0;

        private readonly Dictionary<int, ObjectEvent> _prevEvents = new();

        private readonly Queue<Event> _eventQueue = new();

        //[RuntimeInitializeOnLoadMethod]

        public void ChangeMap(string mapName)
        {
            _eventQueue.Enqueue(new Event { Map = new MapEvent { MapName = mapName } });
        }

        public void CustomEvent(string type, ByteString data)
        {
            _eventQueue.Enqueue(new Event { Custom = new CustomEvent { Type = type, Value = data } });
        }

        public void CustomEvent(string type, string data)
        {
            CustomEvent(type, ByteString.CopyFromUtf8(data));
        }

        public void CustomEvent(string type, byte[] data)
        {
            CustomEvent(type, ByteString.CopyFrom(data));
        }

        private void Start()
        {
            // kill self if other instances of object exist
            var others = FindObjectsOfType<AnalyticsManager>();
            foreach (var other in others)
            {
                if (other.gameObject == gameObject) continue;
                return;
            }

            if (gameVersion == "")
                gameVersion = Application.version;

            StartLogging();
        }

        public void StartLogging()
        {
            var time = DateTimeOffset.Now;
            NewLogFile(time);

            WriteLog(new Game
            {
                AnalyticsVersion = _loggingVersion,
                GameVersion = gameVersion,
                GameTime = ((DateTimeOffset)time.UtcDateTime).ToUnixTimeMilliseconds(),
                Metadata = ""
            }.ToByteArray());

            StartCoroutine(UpdateLog());
        }

        private void WriteLog(byte[] data)
        {
            var length = BitConverter.GetBytes(Convert.ToUInt16(data.Length));
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(length);
            _loggingFile.Write(length);
            _loggingFile.Write(data);

            _loggingFile.Flush(); // maybe dont flush after every write
        }

        private void NewLogFile(DateTimeOffset time)
        {
            CloseLogFile();
            var logFile = "log-" + time.ToString("yyyy-MM-dd\\THH.mm.ss") + ".artemis";
            Directory.CreateDirectory(logPath);
            _loggingStream = new FileStream(Path.Combine(logPath, logFile), FileMode.Append);
            _loggingFile = new BinaryWriter(_loggingStream, Encoding.UTF8);
            _loggingFile.Write(_magic);
            _loggingFile.Flush();
        }

        private void CloseLogFile()
        {
            _loggingFile?.Close();
            _loggingFile = null;

            _loggingStream?.Close();
            _loggingStream = null;
        }

        private void OnDestroy()
        {
            CloseLogFile();
        }

        private IEnumerator UpdateLog()
        {
            while (true)
            {
                GatherEvents();
                var analyticsEvent = new AnalyticsEvent
                {
                    EventTine = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds()
                };
                while (_eventQueue.Count > 0)
                {
                    analyticsEvent.Events.Add(_eventQueue.Dequeue());
                }

                if (analyticsEvent.Events.Count > 0)
                    WriteLog(analyticsEvent.ToByteArray());

                yield return new WaitForSeconds(1f / loggingFPS);
            }
        }

        private void GatherEvents()
        {
            var loggableObjects = FindObjectsOfType<Loggable>();
            foreach (var loggableObject in loggableObjects)
            {
                var unique = false;

                var loggablePosition = loggableObject.transform.position;
                var loggableRotation = loggableObject.transform.rotation;
                var objectEvent = new ObjectEvent
                {
                    Id = GetId(loggableObject.Name()),
                };

                if (!_prevEvents.ContainsKey(objectEvent.Id))
                    _prevEvents.Add(objectEvent.Id, objectEvent);
                var old = _prevEvents[objectEvent.Id];

                var pos = new Position
                {
                    X = loggablePosition.x,
                    Y = loggablePosition.y,
                    Z = loggablePosition.z,
                };
                if (!pos.Equals(old.Position))
                {
                    unique = true;
                    objectEvent.Position = pos;
                    old.Position = pos;
                }

                var rot = new Rotation
                {
                    W = loggableRotation.w,
                    X = loggableRotation.x,
                    Y = loggableRotation.y,
                    Z = loggableRotation.z,
                };
                if (!rot.Equals(old.Rotation))
                {
                    unique = true;
                    objectEvent.Rotation = rot;
                    old.Rotation = rot;
                }

                foreach (var trackableScript in loggableObject.gameObject.GetComponents<ITrackableScript>())
                {
                    var newScript = new ObjectScript
                    {
                        Id = GetId(trackableScript.GetAnalyticsName()),
                        Data = trackableScript.GetAnalyticsFields()
                    };

                    var changed = true;
                    foreach (var oldScript in old.Scripts)
                    {
                        if (oldScript.Id != newScript.Id) continue;
                        if (oldScript.Data != newScript.Data)
                        {
                            oldScript.Data = newScript.Data;
                            break;
                        }

                        changed = false;
                        break;
                    }

                    if (!changed) continue;
                    objectEvent.Scripts.Add(newScript);
                }

                if (unique)
                    _eventQueue.Enqueue(new Event { Object = objectEvent });
            }
        }

        private int GetId(string objName)
        {
            if (_idMap.ContainsKey(objName))
                return _idMap[objName];

            _idMap.Add(objName, _currId);

            _eventQueue.Enqueue(new Event
                {
                    Item = new NewItemEvent { Id = _currId, Name = objName }
                }
            );

            return _currId++;
        }
    }
}