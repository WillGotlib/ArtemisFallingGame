using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class ExplodePlayer : MonoBehaviour
    {
        [SerializeField] private Transform parentObject;
        [SerializeField] private Vector2 forceRange = new (1,10);
        [SerializeField] private Vector2 massRange = new (1,1);

        private List<(Transform obj, Transform parent, Vector3 position, Quaternion, Vector3 scale)> _orig;

        private Rigidbody _rb;
        private Collider _col;

        private void Start()
        {
            _orig = new();
            Flatten(parentObject);
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();

            var bodypartLayer = LayerMask.NameToLayer("Bodypart");
            foreach (var value in _orig)
                value.obj.gameObject.layer = bodypartLayer;
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
            
            // freeze parent
            _rb.isKinematic = true;
            _col.enabled = false;
            
            // flatten and yeet objects
            foreach (var (obj, _, _, _, _) in _orig)
            {
                obj.SetParent(parentObject.parent);

                var rb = obj.gameObject.AddComponent<Rigidbody>();
                rb.AddForce(Random.rotation * Vector3.up * Random.Range(forceRange.x, forceRange.y));
                rb.mass = Random.Range(massRange.x, massRange.y);
                //rb.drag = 50;

                obj.gameObject.AddComponent<BoxCollider>();
            }
        }

        public void ResetExplosion()
        {
            // resume parent
            _rb.isKinematic = false;
            _col.enabled = true;
            
            // reset positions
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