using System;
using protoBuff;
using UnityEngine;

namespace Online
{
    public class Factory : MonoBehaviour
    {
        private Entity _factoryEntity;
        private NetworkedElement _factoryObject;
        private Vector2 _factoryPosition;

        public NetworkedElement SpawnElement(Entity entity, GameObject obj)
        {
            _factoryEntity = entity;
            _factoryPosition = new Vector2 { x = _factoryEntity.Position.X, y = _factoryEntity.Position.Y };


            Vector3 pos = _factoryPosition;
            (pos.y, pos.z) = (pos.z, pos.y);
            var o = Instantiate(obj, pos, new Quaternion());
            _factoryObject = o.GetComponent<NetworkedElement>();
            return _factoryObject;
        }

        public void Update()
        {
            if (_factoryObject == null)
            {
                Debug.Log("factory does not have an object");
            }

            try
            {
                _factoryObject?.HandleUpdate(_factoryPosition, _factoryEntity.Data);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return;
            }

            Destroy(gameObject);
        }
    }
}