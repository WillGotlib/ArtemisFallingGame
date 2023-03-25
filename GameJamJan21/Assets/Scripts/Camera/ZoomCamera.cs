using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private StartGame startGame;
    private Transform player_one;
    private Transform player_two;
    private float orig_dist = 0f;
    private float orig_camera_size = 0f;
    [SerializeField] private float fadingSpeed = 2.0f;
    [SerializeField] private float transitionSpeed = 3.0f;
    private Vector3 orig_pos;
    private Vector3 pos;
    public bool is_zoomed = false;
    [SerializeField] private float camera_x_offset = 4.0f;
    [SerializeField] private Camera main_camera;
    [SerializeField] private float trigger_movement = 0.85f;
    [SerializeField] private float max_zoom = 0.7f;
    private float scalingFactor;
    private bool isCameraSet = false;
    // Start is called before the first frame update
    void Start()
    {
        startGame = FindObjectOfType<StartGame>();
        orig_camera_size = main_camera.orthographicSize;
        orig_pos = transform.position;
    }

    private void LateUpdate()
    {
        pos = transform.position;
        foreach (Transform player in startGame.transform) {
            if (!player_one) {
                player_one = player;
            }
            else if (!player_two) {
                player_two = player;
                orig_dist = Vector3.Distance(player_one.transform.position, player_two.transform.position);
                // Debug.Log(orig_dist);
            }
        }
        
        // Temporary hardcoding'
        if (isCameraSet == false) {
            Vector3 criteriaDistOne = new Vector3(-0.44446f, 1, -3.68f);
            Vector3 criteriaDistTwo = new Vector3(-0.41558f, 1, 2.92f);
            float distScaling = Vector3.Distance(criteriaDistOne, criteriaDistTwo);
            scalingFactor = orig_dist / distScaling;
            orig_camera_size = orig_camera_size * scalingFactor * 0.7f;
            if (orig_camera_size > 10) {
                orig_camera_size = 10f;
            }
            else if (orig_camera_size < 4) {
                orig_camera_size = 4f;
            }
            isCameraSet = true;
        }

        if (player_two) 
            AdjustCamera(player_one, player_two);
    }

    private void AdjustCamera(Transform player_one, Transform player_two) {
        float new_dist = Vector3.Distance(player_one.transform.position, player_two.transform.position);
        if (orig_camera_size * (new_dist/orig_dist) < trigger_movement * orig_camera_size) {
            Vector3 mid_pos = Vector3.Lerp(player_one.transform.position, player_two.transform.position, 0.5f);
            if (mid_pos.x + camera_x_offset > pos.x) {
                pos.x = Mathf.Min(mid_pos.x + camera_x_offset, pos.x + transitionSpeed * Time.deltaTime);
                if (mid_pos.z > pos.z) {
                    pos.z = Mathf.Min(mid_pos.z, pos.z + transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
                else {
                    pos.z = Mathf.Max(mid_pos.z, pos.z - transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
            }
            else {
                pos.x = Mathf.Max(mid_pos.x + camera_x_offset, pos.x - transitionSpeed * Time.deltaTime);
                if (mid_pos.z > pos.z) {
                    pos.z = Mathf.Min(mid_pos.z, pos.z + transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
                else {
                    pos.z = Mathf.Max(mid_pos.z, pos.z - transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
            }
            main_camera.orthographicSize = Mathf.Max(max_zoom * orig_camera_size, orig_camera_size * (new_dist/orig_dist), main_camera.orthographicSize - fadingSpeed * Time.deltaTime);
            is_zoomed = true;
            // pos.x = mid_pos.x + camera_x_offset;
            // pos.z = mid_pos.z;
            // transform.position = pos;
        }
        else {
            if (orig_pos.x > pos.x) {
                pos.x = Mathf.Min(orig_pos.x, pos.x + transitionSpeed * Time.deltaTime);
                if (orig_pos.z > pos.z) {
                    pos.z = Mathf.Min(orig_pos.z, pos.z + transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
                else {
                    pos.z = Mathf.Max(orig_pos.z, pos.z - transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
            }
            else {
                pos.x = Mathf.Max(orig_pos.x, pos.x - transitionSpeed * Time.deltaTime);
                if (orig_pos.z > pos.z) {
                    pos.z = Mathf.Min(orig_pos.z, pos.z + transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
                else {
                    pos.z = Mathf.Max(orig_pos.z, pos.z - transitionSpeed * Time.deltaTime);
                    transform.position = pos;
                }
            }
            main_camera.orthographicSize = Mathf.Min(orig_camera_size, main_camera.orthographicSize + fadingSpeed * Time.deltaTime);
            is_zoomed = false;
        }
    }
}
