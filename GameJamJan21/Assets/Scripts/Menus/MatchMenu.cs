using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchMenu : MatchSetupMenu
{    
    [Header("Starting Match Menu")]
    [SerializeField] GameObject buttonSetParent;
    [SerializeField] GameObject[] playerOptionsParents;
    [SerializeField] GameObject[] playerOptionsSections;

    [SerializeField] Button playButton;

    void Start() {

        for (int i = 0; i < mds.numPlayers; i++) { // Two for the players, two for the options (color and secondary type)
            Button[] optionsButtons = playerOptionsParents[i].GetComponentsInChildren<Button>();
            Color temp = mds.primaryColours[mds.playerColourSchemes[i]];
            temp.a = 1f;
            optionsButtons[0].GetComponent<Image>().color = temp;
            
            optionsButtons[1].GetComponent<Image>().sprite = mds.secondaryTypes[mds.playerSecondaries[i]].GetComponent<BulletLogic>().thumbnail;
            optionsButtons[1].gameObject.GetComponentInChildren<TMP_Text>().text = 
                mds.secondaryTypes[mds.playerSecondaries[i]].GetComponent<BulletLogic>().label;
        }
        initialColors();

        RefreshPlayerSections(mds.numPlayers);
    }
    
    public override void ChooseLevel(int levelNumber) {
        mds.levelIdx = levelNumber;
        print("Selected " + levelNumber);

        // Find the disabled level and bring it back
        Button[] levelButtonSet = buttonSetParent.GetComponentsInChildren<Button>();
        foreach (Button level in levelButtonSet) {
            if (level.interactable == false) {
                level.interactable = true;
            }
        }
    }

    public override void PlayGame() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }
        SceneManager.LoadScene("Gameplay");
    }

    public void RefreshPlayerSections(int numPlayers) {
        for (int i = 0; i < numPlayers; i++) {
            playerOptionsSections[i].SetActive(true);
        }
        if (!playButton.interactable && numPlayers >= 2) {
            playButton.interactable = true;
        }
    }
}
