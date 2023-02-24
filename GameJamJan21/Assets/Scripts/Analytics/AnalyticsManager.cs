using System.Collections.Generic;
using UnityEngine;
using Event = Analytics.AnalyticsEvent.Types.Event;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] trackedTypes;

        private Dictionary<string, int> _idMap = new();

        private void Start()
        {
            var thing = trackedTypes[0];
            var ss = FindObjectsOfType(thing.GetType());
            Debug.Log(ss);
            Debug.Log(GatherEvents());
        }

        private IEnumerable<ITrackableScript> GetTrackedFields(GameObject obj)
        {
            return obj.GetComponents<ITrackableScript>();
        }

        private List<Event> GatherEvents()
        {
            var events = new List<Event>();

            events.Add(new Event
                {
                    Item = new NewItemEvent { Id = 3, Name = "thing" }
                }
            );

            return events;
        }
    }
}