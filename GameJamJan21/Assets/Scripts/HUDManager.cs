using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

//    private int playerCount = 2;
    [SerializeField] private Slider playerOneSlider;
    [SerializeField] private Slider playerTwoSlider;
    private int[] playerStocks = new int[2];
    private float[] playerHealths = new float[2];

    void Start() {
        playerHealths[0] = GlobalStats.baseHealth;
        playerHealths[1] = GlobalStats.baseHealth;
        playerOneSlider.value = GlobalStats.baseHealth;
        playerTwoSlider.value = GlobalStats.baseHealth;
    }

    public void LateUpdate()
    {
        playerOneSlider.value = Mathf.RoundToInt(playerHealths[0]);
        playerTwoSlider.value = Mathf.RoundToInt(playerHealths[1]);
    }

    public void ChangeHealth(int playerNumber, float newHealth) {
        playerHealths[playerNumber] = newHealth;
    }

    public void ChangeStock(int playerNumber, int newStock) {
        playerStocks[playerNumber] = newStock;
    }
}
