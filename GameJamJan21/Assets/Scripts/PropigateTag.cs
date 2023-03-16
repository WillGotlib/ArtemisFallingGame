using System.Collections.Generic;
using UnityEngine;

public class PropigateTag : MonoBehaviour
{
    [SerializeField] private string _tag;

    private void Awake()
    {
        var items = new Queue<GameObject>();
        items.Enqueue(gameObject);
        while (items.Count!=0)
        {
            var item = items.Dequeue();
            item.tag = _tag;
            foreach (Transform child in item.transform)
                items.Enqueue(child.gameObject);
        }
    }
}
