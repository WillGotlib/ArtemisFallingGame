using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera[] cameras;
    private int choice = 0;

    // Start is called before the first frame update
    void Start()
    {
        DisableObjects();
        cameras[choice].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            DisableObjects();
            choice += 1;
            if (choice >= cameras.Length)
            {
                choice = 0;
            }

            cameras[choice].gameObject.SetActive(true);
        }
    }

    public void SwitchCamera()
    {
        DisableObjects();
        choice += 1;
        if (choice >= cameras.Length)
        {
            choice = 0;
        }

        cameras[choice].gameObject.SetActive(true);
    }

    void DisableObjects()
    {
        Camera[] allCameras = Camera.allCameras;
        for (int i = 0; i < allCameras.Length; i++)
        {
            allCameras[i].gameObject.SetActive(false);
        }
    }
}