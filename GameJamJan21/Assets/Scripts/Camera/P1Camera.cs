using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class P1Camera : MonoBehaviour
{
    private DynamicCameraForPlayers mainCamera;
    private CinemachineVirtualCamera vcam;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<DynamicCameraForPlayers>();
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vcam.Follow == null) {
            if (mainCamera.player_one != null) {
                vcam.Follow = mainCamera.player_one;
            }
        }
        if (vcam.LookAt == null) {
            if (mainCamera.player_one != null) {
                vcam.LookAt = mainCamera.player_one;
            }
        }
    }
}
