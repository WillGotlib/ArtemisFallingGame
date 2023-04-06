using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DynamicCameraForPlayers : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;
    private StartGame startGame;
    public Transform player_one;
    public Transform player_two;
    public Transform player_three;
    public Transform player_four;
    // Start is called before the first frame update
    void Start()
    {
        startGame = FindObjectOfType<StartGame>();
        cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
        foreach (Transform player in startGame.transform) {
            if (player_one == null) {
                player_one = player;
            }
            else if (player_two == null && player != player_one) {
                player_two = player;
            }
            else if (player_three == null && player != player_one && player != player_two) {
                player_three = player;
            }
            else if (player_four == null && player != player_one && player != player_two && player != player_three) {
                player_four = player;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach (Transform player in startGame.transform) {
            if (player_one == null) {
                player_one = player;
                cinemachineTargetGroup.AddMember(player_one, 2, 5);
            }
            else if (player_two == null) {
                player_two = player;
                cinemachineTargetGroup.AddMember(player_two, 2, 5);
            }
            else if (player_three == null) {
                player_three = player;
                if (player_three != null && cinemachineTargetGroup.FindMember(player_three) == -1) {
                    cinemachineTargetGroup.AddMember(player_three, 2, 5);
                }
            }
            else if (player_four == null) {
                player_four = player;
                if (player_four != null && cinemachineTargetGroup.FindMember(player_four) == - 1) {
                    cinemachineTargetGroup.AddMember(player_four, 2, 5);
                }
            }
        }
    }
}
