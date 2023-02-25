using UnityEngine;

namespace Analytics
{
    public static class Utils
    {
        
        public static string NameObject(GameObject obj)
        {
            return obj.name +" - "+ obj.GetInstanceID();
        }
    }
}