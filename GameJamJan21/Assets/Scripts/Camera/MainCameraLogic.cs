using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCameraLogic : MonoBehaviour
{
    [SerializeField] private MatchDataScriptable matchData;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject cameraMapOne;
    private GameObject cameraMapTwo;
    private GameObject cameraMapThree;
    private GameObject cameraMapFour;

    
    // Start is called before the first frame update
    void Start()
    {
        cameraMapOne = GameObject.Find("VirtualCameraMapOne");
        cameraMapTwo = GameObject.Find("VirtualCameraMapTwo");
        cameraMapThree = GameObject.Find("VirtualCameraMapThree");
        cameraMapFour = GameObject.Find("VirtualCameraMapFour");
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (matchData.levelIdx == 0) {
            virtualCamera.m_Lens.OrthographicSize = 10f;
            transposer.m_MinimumOrthoSize = 10f;
            transposer.m_MaximumOrthoSize = 10f;
            cameraMapTwo.SetActive(false);
            cameraMapThree.SetActive(false);
            cameraMapFour.SetActive(false);
        }
        if (matchData.levelIdx == 1) {
            virtualCamera.m_Lens.OrthographicSize = 6f;
            transposer.m_MinimumOrthoSize = 6f;
            transposer.m_MaximumOrthoSize = 6f;
            cameraMapOne.SetActive(false);
            cameraMapThree.SetActive(false);
            cameraMapFour.SetActive(false);
        }
        if (matchData.levelIdx == 2) {
            virtualCamera.m_Lens.OrthographicSize = 5f;
            transposer.m_MinimumOrthoSize = 5f;
            transposer.m_MaximumOrthoSize = 5f;
            cameraMapTwo.SetActive(false);
            cameraMapOne.SetActive(false);
            cameraMapFour.SetActive(false);
        }
        if (matchData.levelIdx == 3) {
            virtualCamera.m_Lens.OrthographicSize = 5f;
            transposer.m_MinimumOrthoSize = 5f;
            transposer.m_MaximumOrthoSize = 5f;
            cameraMapTwo.SetActive(false);
            cameraMapThree.SetActive(false);
            cameraMapOne.SetActive(false);
        }
    }

}
