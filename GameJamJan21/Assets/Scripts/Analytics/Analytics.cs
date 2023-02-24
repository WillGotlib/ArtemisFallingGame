using System.Collections.Generic;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsScript : MonoBehaviour
    {
        [SerializeField] private GameObject[] trackedTypes;

        private void Start()
        {
            var thing = trackedTypes[0];
            var ss = FindObjectsOfType(thing.GetType());
            Debug.Log(ss);
        }

        private IEnumerable<ITrackableScript> GetTrackedFields(GameObject obj)
        {
            return obj.GetComponents<ITrackableScript>();
        }
    }
}