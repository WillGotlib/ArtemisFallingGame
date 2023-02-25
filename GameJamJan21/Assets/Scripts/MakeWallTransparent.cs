using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWallTransparent : MonoBehaviour
{
    public Vector3 offest;
    private float fadingSpeed = 0.1f;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private List<Transform> ObjectToHide = new List<Transform>();
    private List<Transform> ObjectToShow = new List<Transform>();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    private StartGame startGame;
 
    void Start()
    {
        startGame = FindObjectOfType<StartGame>();
    }
 
    private void LateUpdate()
    {
        foreach (Transform player in startGame.transform) {
            ManageBlockingView(player);
            ManageBlockingView(player);
        }
 
        foreach (var obstruction in ObjectToHide)
        {
            HideObstruction(obstruction);
        }
 
        foreach (var obstruction in ObjectToShow)
        {
            ShowObstruction(obstruction);
        }
    }
 
    void Update()
    {
     
    }
   
    void ManageBlockingView(Transform player)
    {
        Vector3 playerPosition = player.transform.position + offest;
        float characterDistance = Vector3.Distance(transform.position, playerPosition);
        int layerNumber = LayerMask.NameToLayer("Temp");
        int layerMask = 1 << layerNumber;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, playerPosition - transform.position, characterDistance, layerMask);
        if (hits.Length > 0)
        {
            // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
            foreach (var obstruction in ObjectToHide)
            {
                ObjectToShow.Add(obstruction);
            }
 
            ObjectToHide.Clear();
 
            // Hide the current obstructions
            foreach (var hit in hits)
            {
                Transform obstruction = hit.transform;
                ObjectToHide.Add(obstruction);
                ObjectToShow.Remove(obstruction);
                SetModeTransparent(obstruction);
            }
        }
        else
        {
            // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time
           
            foreach (var obstruction in ObjectToHide)
            {
                ObjectToShow.Add(obstruction);
            }
 
            ObjectToHide.Clear();
 
        }
    }
 
    private void HideObstruction(Transform obj)
    {
        //obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Max(0, color.a - fadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;
 
    }
 
    private void SetModeTransparent(Transform tr)
    {
        MeshRenderer renderer = tr.GetComponent<MeshRenderer>();
        Material originalMat = renderer.sharedMaterial;
        if (!originalMaterials.ContainsKey(tr))
        {
            originalMaterials.Add(tr, originalMat);
        }
        else
        {
            return;
        }
        Material materialTrans = new Material(transparentMaterial);
        //materialTrans.CopyPropertiesFromMaterial(originalMat);
        renderer.material = materialTrans;
        renderer.material.mainTexture = originalMat.mainTexture;
    }
 
    private void SetModeOpaque(Transform tr)
    {
        if (originalMaterials.ContainsKey(tr))
        {
            tr.GetComponent<MeshRenderer>().material = originalMaterials[tr];
            originalMaterials.Remove(tr);
        }
 
    }
 
    private void ShowObstruction(Transform obj)
    {
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Min(1, color.a + fadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;
        if (Mathf.Approximately(color.a, 1f))
        {
            SetModeOpaque(obj);
        }
    }
}