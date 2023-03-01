using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

//    private int playerCount = 2;
    [SerializeField] private Slider playerOneHealth;
    [SerializeField] private Slider playerTwoHealth;
    [SerializeField] private Slider playerOneStamina;
    [SerializeField] private Slider playerTwoStamina;
    private float playerOneWaitTime = 5.0f;
    private float playerTwoWaitTime = 5.0f;
    private int[] playerStocks = new int[2];
    private float[] playerHealths = new float[2];

    void Start() {
        playerHealths[0] = GlobalStats.baseHealth;
        playerHealths[1] = GlobalStats.baseHealth;
        playerOneHealth.value = GlobalStats.baseHealth;
        playerTwoHealth.value = GlobalStats.baseHealth;
        playerOneStamina.value = 100;
        playerTwoStamina.value = 100;
    }

    void Update() {
        if (playerOneStamina.value != 100) {
            playerOneWaitTime -= Time.deltaTime;
            playerOneStamina.value = (GlobalStats.dashCooldown - playerOneWaitTime) * 20;
        }
        if (playerTwoStamina.value != 100) {
            playerTwoWaitTime -= Time.deltaTime;
            playerTwoStamina.value = (GlobalStats.dashCooldown - playerTwoWaitTime) * 20;
        }
    }

    public void LateUpdate()
    {
        playerOneHealth.value = Mathf.RoundToInt(playerHealths[0]);
        playerTwoHealth.value = Mathf.RoundToInt(playerHealths[1]);
    }

    public void ChangeHealth(int playerNumber, float newHealth) {
        playerHealths[playerNumber] = newHealth;
    }

    public void ChangeStock(int playerNumber, int newStock) {
        playerStocks[playerNumber] = newStock;
    }
    
    public void UseStamina(int playerNumber) {
        if (playerNumber == 0) {
            playerOneStamina.value = 0;
            playerOneWaitTime = 5.0f;
        }

        else if (playerNumber == 1) {
            playerTwoStamina.value = 0;
            playerTwoWaitTime = 5.0f;
        }
    }
}
