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
    private float fadingSpeed = 0.5f;
    private Vector3 orig_pos;
    private Vector3 pos;
    [SerializeField] private float camera_x_offset = 4.0f;
    [SerializeField] private Camera main_camera;
    [SerializeField] private float trigger_movement = 0.5f;
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
            if (player_one == null) {
                player_one = player;
            }
            else if (player_two == null) {
                player_two = player;
                orig_dist = Vector3.Distance(player_one.transform.position, player_two.transform.position);
                Debug.Log(orig_dist);
            }
        }

        AdjustCamera(player_one, player_two);
    }

    private void AdjustCamera(Transform player_one, Transform player_two) {
        float new_dist = Vector3.Distance(player_one.transform.position, player_two.transform.position);
        if (orig_camera_size * (new_dist/orig_dist) < trigger_movement * orig_camera_size) {
            main_camera.orthographicSize = Mathf.Max(orig_camera_size * (new_dist/orig_dist), main_camera.orthographicSize - fadingSpeed * Time.deltaTime);
            Vector3 mid_pos = Vector3.Lerp(player_one.transform.position, player_two.transform.position, 0.5f);
            pos.x = mid_pos.x + camera_x_offset;
            pos.z = mid_pos.z;
            transform.position = pos;
        }
        else {
            main_camera.orthographicSize = Mathf.Min(orig_camera_size, main_camera.orthographicSize + fadingSpeed * Time.deltaTime);
            transform.position = orig_pos;
        }
    }
}
