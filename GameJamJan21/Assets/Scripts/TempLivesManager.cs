using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempLivesManager : MonoBehaviour
{
    [SerializeField] private Image liveOne;
    [SerializeField] private Image liveTwo;
    [SerializeField] private Image liveThree;
    [SerializeField] private Image liveFour;
    [SerializeField] private Image liveFive;
    [SerializeField] private Image liveSix;

    // Start is called before the first frame update
    void Start()
    {
        ResetDeath();
    }

    public void ApplyDeath(int playerNumber) {
        if (playerNumber == 0) {
            if (liveOne.enabled == true) {
                liveOne.enabled = false;
            }
            else if (liveTwo.enabled == true) {
                liveTwo.enabled = false;
            }
            else if (liveThree.enabled == true) {
                liveThree.enabled = false;
            }
        }

        else if (playerNumber == 1) {
            if (liveFour.enabled == true) {
                liveFour.enabled = false;
            }
            else if (liveFive.enabled == true) {
                liveFive.enabled = false;
            }
            else if (liveSix.enabled == true) {
                liveSix.enabled = false;
            }
        }
    }

    public void ResetDeath() {
        liveOne.enabled = true;
        liveTwo.enabled = true;
        liveThree.enabled = true;
        liveFour.enabled = true;
        liveFive.enabled = true;
        liveSix.enabled = true;

        if (GlobalStats.defaultStockCount < 4) {
            liveThree.enabled = false;
            liveSix.enabled = false;
        }
        if (GlobalStats.defaultStockCount < 3) {
            liveTwo.enabled = false;
            liveFive.enabled = false;
        }
        if (GlobalStats.defaultStockCount < 2) {
            liveOne.enabled = false;
            liveFour.enabled = false;
        }
    }
}
