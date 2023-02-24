using UnityEngine;

namespace Analytics
{
    public class Loggable : MonoBehaviour
    {
        public string Name()
        {
            return gameObject.name +" - "+ GetInstanceID();
        }
    }
}