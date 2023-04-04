using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public MatchDataScriptable mds;
    
    private float[] playerHealths;
    [SerializeField] private Slider[] playerHealthBars;
    private float[] playerWaitTimes;
    [SerializeField] private Slider[] playerStaminaBars;
    private int[] playerStocks;

    
    private int staminaDisplayMultiplier = 30;

    void Start() {
        playerStocks = new int[mds.numPlayers];
        playerHealths = new float[mds.numPlayers];
        playerWaitTimes = new float[mds.numPlayers];

        for (int i = 0; i < mds.numPlayers; i++) {
            playerHealths[i] = GlobalStats.baseHealth;
            playerHealthBars[i].value = playerHealths[i];
            playerStaminaBars[i].value = 100;
            playerWaitTimes[i] = GlobalStats.dashCooldown;
        }
    }

    public void InitHealth() {
        playerStocks = new int[mds.numPlayers];
        playerHealths = new float[mds.numPlayers];

        for (int i = 0; i < mds.numPlayers; i++) {
            playerHealths[i] = GlobalStats.baseHealth;
            playerHealthBars[i].value = playerHealths[i];
            playerStaminaBars[i].value = 100;
        }
    }

    void Update() {
        for (int i = 0; i < mds.numPlayers; i++) {
            if (playerStaminaBars[i].value != 0) {
                playerWaitTimes[i] -= Time.deltaTime;
                playerStaminaBars[i].value = (GlobalStats.dashCooldown - playerWaitTimes[i]) * staminaDisplayMultiplier;
            }
            playerHealthBars[i].value = playerHealths[i];
        }
    }

    public void LateUpdate()
    {
        for (int i = 0; i < mds.numPlayers; i++) {
            playerHealthBars[i].value = playerHealths[i];
        }
    }

    public void ChangeHealth(int playerNumber, float newHealth) {
        playerHealths[playerNumber] = newHealth;
    }

    public void ChangeStock(int playerNumber, int newStock) {
        playerStocks[playerNumber] = newStock;
    }
    
    public void UseStamina(int playerNumber) {
        for (int i = 0; i < mds.numPlayers; i++) {
            playerStaminaBars[i].value = 0;
            playerWaitTimes[i] = GlobalStats.dashCooldown;
        }
    }
}
