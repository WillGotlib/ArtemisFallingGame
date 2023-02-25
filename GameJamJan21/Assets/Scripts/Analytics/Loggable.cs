using UnityEngine;

namespace Analytics
{
    public class Loggable : MonoBehaviour
    {
        public string Name()
        {
            return Utils.NameObject(gameObject);
        }
    }
}