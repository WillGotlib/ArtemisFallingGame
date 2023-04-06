using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxParalax : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float skyboxScale = 1f;

    private Vector3 _mainCamStartPos;
    private Vector3 _skyboxCamStartPos;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        _mainCamStartPos = mainCamera.transform.position;
        _skyboxCamStartPos = transform.position;
    }
    
    void Update()
    {
        var mainCamDeltaPos = mainCamera.transform.position - _mainCamStartPos;
        transform.position = _skyboxCamStartPos + mainCamDeltaPos * skyboxScale;

        transform.rotation = mainCamera.transform.rotation;
    }
}
