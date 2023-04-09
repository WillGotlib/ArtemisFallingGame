using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class ExplodePlayer : MonoBehaviour
    {
        [SerializeField] private Transform parentObject;
        [SerializeField] private float minForce = 1;
        [SerializeField] private float maxForce = 10;

        private List<(Transform obj, Transform parent, Vector3 position, Quaternion, Vector3 scale)> _orig;

        private void Start()
        {
            _orig = new();
            Flatten(parentObject);
            //ResetExplosion();
        }

#if UNITY_EDITOR && false // for testing
        private bool ex;

        private void Update()
        {
            if (!ex && Input.GetKeyUp(KeyCode.K))
            {
                ex = true;
                Explode();
            }
            else if (ex && Input.GetKeyUp(KeyCode.L))
            {
                ex = false;
                ResetExplosion();
            }
        }
#endif

        public void Explode()
        {
            // special sound for explosion

            foreach (var (obj, _, _, _, _) in _orig)
            {
                obj.SetParent(parentObject.parent);

                var rb = obj.gameObject.AddComponent<Rigidbody>();
                rb.AddForce(Random.rotation * Vector3.up * Random.Range(minForce, maxForce));
                obj.gameObject.AddComponent<BoxCollider>();
            }
        }

        public void ResetExplosion()
        {
            foreach (var (obj, parent, vector3, quaternion, scale) in _orig)
            {
                Destroy(obj.gameObject.GetComponent<Rigidbody>());
                Destroy(obj.gameObject.GetComponent<BoxCollider>());

                obj.SetParent(parent);
                obj.localPosition = vector3 + Vector3.zero;
                obj.localRotation = quaternion * Quaternion.identity;
                obj.localScale = scale;
            }
        }

        private void Flatten(Transform obj)
        {
            foreach (Transform o in obj)
            {
                Flatten(o);
            }

            _orig.Add((obj, obj.parent, obj.localPosition + Vector3.zero, obj.localRotation * Quaternion.identity,
                obj.localScale + Vector3.zero));
        }
    }
}