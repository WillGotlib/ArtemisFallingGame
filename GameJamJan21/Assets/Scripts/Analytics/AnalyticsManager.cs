using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Event = Analytics.AnalyticsEvent.Types.Event;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private string _loggingVersion = "0.0.1";

        [SerializeField] private int loggingFPS = 30;
        [SerializeField] private string gameVersion;

        [SerializeField] private string logPath = "GameLogs/";

        private FileStream _loggingStream;
        private BinaryWriter _loggingFile;

        private readonly Dictionary<string, int> _idMap = new();
        private int _currId = 0;

        private readonly Dictionary<int, Event> _prevEvents = new(); //todo save bandwidth by only saving changed events
        private readonly Queue<Event> _eventQueue = new();

        //[RuntimeInitializeOnLoadMethod]

        public void ChangeMap(string mapName)
        {
            _eventQueue.Enqueue(new Event{Map = new MapEvent{MapName = mapName}});
        }

        private void Start()
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
        }

        private void NewLogFile(DateTimeOffset time)
        {
            CloseLogFile();
            var logFile = "log-" + time.ToString("yyyy-MM-dd\\THH.mm.ss") + ".artemis";
            Directory.CreateDirectory(logPath);
            _loggingStream = new FileStream(Path.Combine(logPath, logFile), FileMode.Append);
            _loggingFile = new BinaryWriter(_loggingStream, Encoding.UTF8);
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
                while (_eventQueue.Count>0)
                {
                    WriteLog(_eventQueue.Dequeue().ToByteArray());
                }

                yield return new WaitForSeconds(1f / loggingFPS);
            }
        }

        private IEnumerable<ITrackableScript> GetTrackedFields(GameObject obj)
        {
            return obj.GetComponents<ITrackableScript>();
        }

        private void GatherEvents()
        {
            var loggableObjects = FindObjectsOfType<Loggable>();
            foreach (var loggableObject in loggableObjects)
            {
                var scripts = new RepeatedField<ObjectScript>();
                foreach (var trackableScript in loggableObject.gameObject.GetComponents<ITrackableScript>())
                {
                    scripts.Add(
                        new ObjectScript
                        {
                          Id  = GetId(trackableScript.GetName()),
                          Data = trackableScript.GetFields()
                        });
                }

                var loggablePosition = loggableObject.transform.position;
                var loggableRotation = loggableObject.transform.rotation;
                _eventQueue.Enqueue(new Event
                    {
                        Object = new ObjectEvent
                        {
                            Id = GetId(loggableObject.Name()),
                            Position = new Position
                            {
                                X=loggablePosition.x,
                                Y=loggablePosition.y,
                                Z=loggablePosition.z,
                            },
                            Rotation = new Rotation
                            {
                                W=loggableRotation.w,
                                X=loggableRotation.x,
                                Y=loggableRotation.y,
                                Z=loggableRotation.z,
                            },
                            Scripts = {scripts}
                        }
                    }
                );
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