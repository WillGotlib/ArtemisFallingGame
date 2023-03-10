using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public bool activated;

    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (activated) {
            // print("ACTIVE");
            DynamicAction();
        }
    }

    public abstract void DynamicAction();
}
