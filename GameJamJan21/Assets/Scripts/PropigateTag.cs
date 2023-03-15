using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropigateTag : MonoBehaviour
{
    [SerializeField] private string tag;

    private void Awake()
    {
        var items = new Queue<GameObject>();
        items.Enqueue(gameObject);
        while (items.Count!=0)
        {
            var item = items.Dequeue();
            item.tag = tag;
            foreach (Transform child in item.transform)
                items.Enqueue(child.gameObject);
        }
    }
}
