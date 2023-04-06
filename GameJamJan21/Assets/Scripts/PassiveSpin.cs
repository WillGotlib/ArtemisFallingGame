using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSpin : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(30f * Time.deltaTime, 60f * Time.deltaTime, 10f * Time.deltaTime);
    }
}
