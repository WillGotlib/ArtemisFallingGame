using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using UnityEngine;
using protoBuff;
using Request = protoBuff.Request;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

//todo add try catches in places to get errors
// todo make sure that there is a connection / detect disconnect
namespace Online
{
    public class NetworkManager : MonoBehaviour
    {
        public GameObject[] spawnables;
        private Dictionary<string, GameObject> _spawnables;

        public int updateFps = 60; // update at 60 fps

        private readonly Dictionary<string, NetworkedElement> _objects;
        private readonly Dictionary<string, Vector2> _objectLastPos;

        private delegate void RunOnMainthread();

        private readonly Queue<RunOnMainthread> _mainthreadQueue;

        public NetworkManager()

        {
            _objects = new Dictionary<string, NetworkedElement>();
            _objectLastPos = new Dictionary<string, Vector2>();
            _mainthreadQueue = new Queue<RunOnMainthread>();
            GRPC.RegisterMessageCallback(onMessage);
        }

        // Start is called before the first frame update
        public void Start()
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
                _mainthreadQueue.Dequeue()();
            }
        }

        /// be careful with this and dont have scripts register on wake since it can lead to recursion 
        public void RegisterObject(NetworkedElement obj)
        {
            var id = Guid.NewGuid().ToString();
            _objects.Add(id, obj);
            PostRegistration(id, obj);
        }

        public void UnregisterObject(NetworkedElement obj)
        {
            var id = "";
            foreach (var (uid, element) in _objects)
            {
                if (!element.Equals(obj)) continue;
                id = uid;
                break;
            }

            UnregisterObject(id);
        }

        public void UnregisterObject(string id)
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
            GRPC.SendRequest(req);
        }

        public void Connect(string sessionID)
        {
            RepeatedField<Entity> entities;
            try
            {
                entities = GRPC.Connect(sessionID);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Unknown) Debug.LogWarning(e.Status.Detail);
                return;
            }

            Debug.Log(entities);

            foreach (var entity in entities)
            {
                AddEntity(entity);
            }

            try
            {
                GRPC.StartStream();
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Unknown) Debug.LogWarning(e.Status.Detail);
                return;
            }

            PostRegistrers();

            StartCoroutine(UpdatePosition());
        }

        private void onMessage(Response action)
        {
            foreach (var response in action.Responses)
            {
                Debug.Log(action);
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
            var objectID = "";
            foreach (var (id, element) in _objects)
            {
                if (element != obj) continue;
                objectID = id;
                break;
            }

            if (objectID == "") throw new Exception("Cant update, not registered");

            GRPC.SendRequest(new StreamAction
            {
                UpdateEntity = new UpdateEntity
                {
                    Entity = new Entity
                    {
                        Data = obj.Data(),
                        Id = objectID,
                        Position = ToPosition(obj.GetPosition()),
                        Type = obj.ID()
                    }
                }
            });
        }

        private bool isControlled(string id)
        {
            return _objects.ContainsKey(id) && _objects[id].GetControlType() == ElementType.Owner;
        }

        private void AddEntity(Entity entity)
        {
            if (_objects.ContainsKey(entity.Id)) return;
            var factory = new GameObject().AddComponent<Factory>();
            var script = factory.SpawnElement(entity, _spawnables[entity.Type]);
            _objects[entity.Id] = script;
        }

        private void RemoveEntity(string id)
        {
            if (isControlled(id)) return;
            var obj = _objects[id];
            _objects.Remove(id);
            obj.Destroy();
        }

        private void UpdateEntity(Entity entity)
        {
            if (isControlled(entity.Id)) return;
            _objects[entity.Id].HandleUpdate(ToVector2(entity.Position), entity.Data);
        }

        private void MoveEntity(MoveEntity moveAction)
        {
            if (isControlled(moveAction.Id)) return;
            _objects[moveAction.Id].HandleUpdate(ToVector2(moveAction.Position), "");
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public async void Disconnect()
        {
            StopAllCoroutines();
            await Task.Delay((int)(1000f / updateFps) + 10);
            GRPC.Disconnect();
        }

        private static Position ToPosition(Vector2 position)
        {
            return new Position
            {
                X = position.x,
                Y = position.y
            };
        }

        private static Vector2 ToVector2(Position position)
        {
            return new Vector2
            {
                x = position.X,
                y = position.Y
            };
        }

        private void PostRegistrers()
        {
            foreach (var (id, obj) in _objects)
            {
                if (obj.GetControlType() == ElementType.Owner)
                    PostRegistration(id, obj);
            }
        }

        private void PostRegistration(string id, NetworkedElement obj)
        {
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
                        Position = ToPosition(obj.GetPosition())
                    }
                }
            };
            GRPC.SendRequest(req);
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

                    var pos = element.GetPosition();
                    if (_objectLastPos.ContainsKey(id) &&
                        _objectLastPos[id] == pos) continue;
                    _objectLastPos[id] = pos;

                    requests.Add(new StreamAction
                    {
                        MoveEntity = new MoveEntity
                        {
                            Id = id,
                            Position = ToPosition(pos)
                        }
                    });
                }

                GRPC.SendRequest(new Request { Requests = { requests } });

                yield return new WaitForSeconds(1f / updateFps);
            }
        }
    }
}