using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWallTransparent : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float fadingSpeed = 0.8f;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private List<Transform> ObjectToHide = new List<Transform>();
    private ZoomCamera zoomCamera;
    private HashSet<Transform> ObjectToShow = new ();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    private StartGame startGame;
    private Transform player_one;
    private Transform player_two;
 
    void Start()
    {
        startGame = FindObjectOfType<StartGame>();
        zoomCamera = FindObjectOfType<ZoomCamera>();
    }
 
    private void LateUpdate()
    {
        // Keep up the position of players

        foreach (Transform player in startGame.transform) {
            if (player_one == null) {
                player_one = player;
            }
            else {
                player_two = player;
            }
        }

        // update objects that are blocking and showing
        if (zoomCamera.is_zoomed && player_two) {
            Vector3 new_offset = offset;
            new_offset.x = offset.x - 0.5f;
            ManageBlockingView(player_one, player_two, new_offset);
        }
        else if (player_two) {
            ManageBlockingView(player_one, player_two, offset);
        }
        
        // hide the obstacles
        foreach (var obstruction in ObjectToHide)
        {
            HideObstruction(obstruction);
        }

        // show obstacles
        foreach (var obstruction in new List<Transform>(ObjectToShow))
        {   
            if (obstruction != null) {
                ShowObstruction(obstruction);
            }
        }
    }
 
   
    void ManageBlockingView(Transform player_one, Transform player_two, Vector3 offset)
    {
        Vector3 playerOnePosition = player_one.transform.position + offset;
        Vector3 playerTwoPosition = player_two.transform.position + offset;
        float characterDistanceOne = Vector3.Distance(transform.position, playerOnePosition);
        float characterDistanceTwo = Vector3.Distance(transform.position, playerTwoPosition);
        int layerNumber = LayerMask.NameToLayer("Obstacle");
        int layerMask = 1 << layerNumber;
        RaycastHit[] hitsOne = Physics.RaycastAll(transform.position, playerOnePosition - transform.position, characterDistanceOne, layerMask);
        RaycastHit[] hitsTwo = Physics.RaycastAll(transform.position, playerTwoPosition - transform.position, characterDistanceTwo, layerMask);
        if (hitsOne.Length > 0 || hitsTwo.Length > 0)
        {
            // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
            foreach (var obstruction in ObjectToHide)
            {
                ObjectToShow.Add(obstruction);
            }
 
            ObjectToHide.Clear();
 
            // Hide the current obstructions
            foreach (var hitOne in hitsOne)
            {
                Transform obstruction = hitOne.transform;
                ObjectToHide.Add(obstruction);
                ObjectToShow.Remove(obstruction);
                SetModeTransparent(obstruction);
            }
            foreach (var hitTwo in hitsTwo) 
            {
                Transform obstruction = hitTwo.transform;
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
        color.a = Mathf.Max(0.5f, color.a - fadingSpeed * Time.deltaTime);
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
        // materialTrans.CopyPropertiesFromMaterial(originalMat);
        renderer.material = materialTrans;
        renderer.material.mainTexture = originalMat.mainTexture;
    }
 
    private void SetModeOpaque(Transform tr)
    {
        if (originalMaterials.ContainsKey(tr))
        {
            tr.GetComponent<MeshRenderer>().material = originalMaterials[tr];
            originalMaterials.Remove(tr);
            ObjectToShow.Remove(tr);
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