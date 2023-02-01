using System;
using protoBuff;
using UnityEngine;

namespace Online
{
    public class Factory : MonoBehaviour
    {
        private NetworkedElement _factoryObject;
        private string _factoryData;
        private Vector3 _factoryPosition;
        private Quaternion _factoryRotation;

        public NetworkedElement SpawnElement(Entity entity, GameObject obj)
        {
            _factoryData = entity.Data;
            _factoryPosition = Helpers.ToVector3(entity.Position);
            _factoryRotation = Helpers.ToQuaternion(entity.Rotation);

            var o = Instantiate(obj, _factoryPosition, _factoryRotation);
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
                _factoryObject?.HandleUpdate(_factoryPosition, _factoryRotation, _factoryData);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return;
            }

            //Destroy(gameObject);
        }
    }
}