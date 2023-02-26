using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private TMP_Text[] playerTexts = {};
    private int playerCount = 2;
    private int[] playerStocks = new int[2];
    private float[] playerHealths = new float[2];

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < playerCount; i++) {
            playerTexts[i].text = $"P{i}: {playerHealths[i]}/{GlobalStats.baseHealth} [{playerStocks[i]}]";
        }
    }

    public void ChangeHealth(int playerNumber, float newHealth) {
        playerHealths[playerNumber] = newHealth;
    }

    public void ChangeStock(int playerNumber, int newStock) {
        playerStocks[playerNumber] = newStock;
    }
}
