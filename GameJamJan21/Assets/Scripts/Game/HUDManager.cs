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
    public MatchDataScriptable mds;
    private float playerOneWaitTime = GlobalStats.dashCooldown;
    private float playerTwoWaitTime = GlobalStats.dashCooldown;
    private int[] playerStocks;
    private float[] playerHealths;
    private int staminaDisplayMultiplier = 30;

    void Start() {
        playerStocks = new int[mds.numPlayers];
        playerHealths = new float[mds.numPlayers];

        var i = 0;
        while (i < playerHealths.Length) {
            playerHealths[i] = GlobalStats.baseHealth;
            i += 1;
        }

        playerOneHealth.value = GlobalStats.baseHealth;
        playerTwoHealth.value = GlobalStats.baseHealth;
        playerOneStamina.value = 100;
        playerTwoStamina.value = 100;
    }

    public void InitHealth() {
        playerStocks = new int[mds.numPlayers];
        playerHealths = new float[mds.numPlayers];

        var i = 0;
        while (i < playerHealths.Length) {
            playerHealths[i] = GlobalStats.baseHealth;
            i += 1;
        }
    }

    void Update() {
        if (playerOneStamina.value != 100) {
            playerOneWaitTime -= Time.deltaTime;
            playerOneStamina.value = (GlobalStats.dashCooldown - playerOneWaitTime) * staminaDisplayMultiplier;
        }
        if (playerTwoStamina.value != 100) {
            playerTwoWaitTime -= Time.deltaTime;
            playerTwoStamina.value = (GlobalStats.dashCooldown - playerTwoWaitTime) * staminaDisplayMultiplier;
        }
    }

    public void LateUpdate()
    {
        // playerOneHealth.value = Mathf.RoundToInt(playerHealths[0]);
        // playerTwoHealth.value = Mathf.RoundToInt(playerHealths[1]);
        playerOneHealth.value = (playerHealths[0]);
        playerTwoHealth.value = (playerHealths[1]);
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
            playerOneWaitTime = GlobalStats.dashCooldown;
        }

        else if (playerNumber == 1) {
            playerTwoStamina.value = 0;
            playerTwoWaitTime = GlobalStats.dashCooldown;
        }
    }
}
