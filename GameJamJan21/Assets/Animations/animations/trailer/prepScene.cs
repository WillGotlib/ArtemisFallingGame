using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class prepScene : MonoBehaviour
{
    public DashJets jets;
    public Volume volume;

    public void Scene2()
    {
        jets.Shoot();
        volume.GetComponent<DepthOfField>().active = true;
    }
}
