using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using protoBuff;
using RSG;
using Unity.WebRTC;
using UnityEngine;

//todo add try catches in places to get errors
// todo make sure that there is a connection / detect disconnect
namespace Online
{
    public class NetworkManager : MonoBehaviour
    {
        public GameObject[] spawnables;
        private Dictionary<string, GameObject> _spawnables;

        public int updateFps = 60; // update at 60 fps

        private readonly Dictionary<ByteString, NetworkedElement> _objects;
        private readonly Dictionary<ByteString, (Vector3, Quaternion, float)> _objectLastPos;
        private const float MaxSecondsNoUpdate = 3; // update every 3 seconds even if no movement 

        private delegate void RunOnMainthread();

        private readonly Queue<RunOnMainthread> _mainthreadQueue;

        private GameObject networkParent;
        private Coroutine positionUpdater;
        private RepeatedField<Entity> onJoinEnteties;
        
        public NetworkManager()

        {
            _objects = new Dictionary<ByteString, NetworkedElement>();
            _objectLastPos = new Dictionary<ByteString, (Vector3, Quaternion, float)>();
            _mainthreadQueue = new Queue<RunOnMainthread>();
            Connection.RegisterMessageCallback(onMessage);
        }

        public void PrepNewScene()
        {
            if (positionUpdater != null) StopCoroutine(positionUpdater);

            networkParent = new GameObject("Networked Objects");
            _objects.Clear();
            _objectLastPos.Clear();
            foreach (var entity in onJoinEnteties)
            {
                AddEntity(entity);
            }
            
            PostRegistrers();

            positionUpdater = StartCoroutine(UpdatePosition());
        }

        public void OnDisconnect(DelegateOnClose disconnect)
        {
            Connection.OnClose(disconnect);
        }

        public void Awake()
        {
            // kill self if other instances of object exist
            var others = FindObjectsOfType<NetworkManager>();
            foreach (var other in others)
            {
                if (other.gameObject == gameObject) continue;
                return;
            }

            _spawnables = new Dictionary<string, GameObject>();
            foreach (var spawnable in spawnables)
            {
                var networkedElement = spawnable.GetComponent<NetworkedElement>();
                if (networkedElement == null)
                    throw new Exception(spawnable.name + " is missing a script that implements NetworkedElement");

                if (_spawnables.ContainsKey(networkedElement.ID()))
                    throw new Exception("name collision with " + networkedElement.ID());
                _spawnables[networkedElement.ID()] = spawnable;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            while (_mainthreadQueue.Count > 0)
            {
                try
                {
                    _mainthreadQueue.Dequeue()();
                }
                catch (MissingReferenceException e)
                {
                    Debug.LogWarning(e);
                }
            }
        }

        /// be careful with this and dont have scripts register on wake since it can lead to recursion 
        public void RegisterObject(NetworkedElement obj)
        {
            var id = Guid.NewGuid().ToByteArray();
            var uid = ByteString.CopyFrom(id);
            _objects.Add(uid, obj);
            PostRegistration(uid, obj);
        }

        public void UnregisterObject(NetworkedElement obj)
        {
            var id = ByteString.Empty;
            foreach (var (uid, element) in _objects)
            {
                if (!element.Equals(obj)) continue;
                id = uid;
                break;
            }

            UnregisterObject(id);
        }

        public void UnregisterObject(ByteString id)
        {
            if (_objects.ContainsKey(id)) return;
            _objects[id].Destroy();
            _objects.Remove(id);

            var req = new StreamAction
            {
                RemoveEntity = new RemoveEntity
                {
                    Id = id
                }
            };
            Connection.SendPriority(req);
        }

        public Promise Connect(string sessionID)
        {
            var result = new Promise();
            Connection.Connect(sessionID).Then(entities =>
            {
                Debug.Log(entities);
                onJoinEnteties = entities;

                Connection.StartStream().Then(result.Resolve).Catch(result.Reject);
            }).Catch(result.Reject);
            return result;
        }

        private void onMessage(Response action)
        {
            foreach (var response in action.Responses) // maybe unwrap events and fire them one by one
            {
                // Debug.Log(action);
                RunOnMainthread function = null;
                switch (response.ActionCase)
                {
                    case StreamAction.ActionOneofCase.AddEntity:
                        function = () => { AddEntity(response.AddEntity.Entity); };
                        break;
                    case StreamAction.ActionOneofCase.RemoveEntity:
                        function = () => { RemoveEntity(response.RemoveEntity.Id); };
                        break;
                    case StreamAction.ActionOneofCase.UpdateEntity:
                        function = () => { UpdateEntity(response.UpdateEntity.Entity); };
                        break;
                    case StreamAction.ActionOneofCase.MoveEntity:
                        function = () => { MoveEntity(response.MoveEntity); };
                        break;
                    case StreamAction.ActionOneofCase.None:
                    default:
                        break;
                }

                if (function != null)
                    _mainthreadQueue.Enqueue(function);
            }
        }

        public void UpdateObject(NetworkedElement obj)
        {
            var objectID = ByteString.Empty;
            foreach (var (id, element) in _objects)
            {
                if (element != obj) continue;
                objectID = id;
                break;
            }

            if (objectID == ByteString.Empty) throw new Exception("Cant update, not registered");

            var pos = obj.GetPosition();
            Connection.SendPriority(new StreamAction
            {
                UpdateEntity = new UpdateEntity
                {
                    Entity = new Entity
                    {
                        Data = obj.Data(),
                        Id = objectID,
                        Position = Helpers.ToPosition(pos.Item1),
                        Rotation = Helpers.ToRotation(pos.Item2),
                        Type = obj.ID()
                    }
                }
            });
        }

        private bool isControlled(ByteString id)
        {
            return _objects.ContainsKey(id) && _objects[id].GetControlType() == ElementType.Owner;
        }

        private void AddEntity(Entity entity)
        {
            if (_objects.ContainsKey(entity.Id)) return;
            var factory = new GameObject().AddComponent<Factory>();
            var script = factory.SpawnElement(entity, _spawnables[entity.Type], networkParent.transform);
            _objects[entity.Id] = script;
        }

        private void RemoveEntity(ByteString id)
        {
            if (isControlled(id)) return;
            var obj = _objects[id];
            _objects.Remove(id);
            obj.Destroy();
        }

        private void UpdateEntity(Entity entity)
        {
            if (isControlled(entity.Id)) return;
            _objects[entity.Id]
                .HandleUpdate(Helpers.ToVector3(entity.Position), Helpers.ToQuaternion(entity.Rotation), entity.Data);
        }

        private void MoveEntity(MoveEntity moveAction)
        {
            if (isControlled(moveAction.Id)) return;
            _objects[moveAction.Id].HandleUpdate(Helpers.ToVector3(moveAction.Position),
                Helpers.ToQuaternion(moveAction.Rotation), "");
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            Application.quitting += Connection.Disconnect;
        }

        public async void Disconnect()
        {
            StopAllCoroutines();
            await Task.Delay((int)(1000f / updateFps) + 10);
            Connection.Disconnect();
        }

        private void PostRegistrers()
        {
            foreach (var (id, obj) in _objects)
            {
                if (obj.GetControlType() == ElementType.Owner)
                    PostRegistration(id, obj);
            }
        }

        private void PostRegistration(ByteString id, NetworkedElement obj)
        {
            var pos = obj.GetPosition();
            var req = new StreamAction
            {
                AddEntity = new AddEntity
                {
                    KeepOnDisconnect = !obj.RemoveOnDisconnect(),
                    Entity = new Entity
                    {
                        Id = id,
                        Type = obj.ID(),
                        Data = obj.Data(),
                        Position = Helpers.ToPosition(pos.Item1),
                        Rotation = Helpers.ToRotation(pos.Item2)
                    }
                }
            };
            Connection.SendPriority(req);
        }

        IEnumerator UpdatePosition()
        {
            while (true)
            {
                var requests = new RepeatedField<StreamAction>();
                foreach (var (id, element) in _objects)
                {
                    if (element.GetControlType() == ElementType.Listener) continue;
                    // ideally projectiles should be controlled by the server but i am making them be controlled by the sender for simplicities sake

                    (Vector3, Quaternion) pos;
                    try
                    {
                        pos = element.GetPosition();
                    }
                    catch
                    {
                        continue; // object was destroyed
                    }

                    if (_objectLastPos.ContainsKey(id) && (Time.time - _objectLastPos[id].Item3) < MaxSecondsNoUpdate &&
                        _objectLastPos[id].Item1 == pos.Item1 && _objectLastPos[id].Item2 == pos.Item2) continue;
                    _objectLastPos[id] = (pos.Item1,pos.Item2, Time.time);

                    requests.Add(new StreamAction
                    {
                        MoveEntity = new MoveEntity
                        {
                            Id = id,
                            Position = Helpers.ToPosition(pos.Item1),
                            Rotation = Helpers.ToRotation(pos.Item2)
                        }
                    });
                }

                if (requests.Count > 0)
                    Connection.SendFast(new Request { Requests = { requests } });
                // there is a change that if something moves in one frame and its packet gets lost but it doesnt move later that it just wont update

                yield return new WaitForSeconds(1f / updateFps);
            }
        }
    }
}