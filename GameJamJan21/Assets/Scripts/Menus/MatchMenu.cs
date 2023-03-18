using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchMenu : MatchSetupMenu
{    
    // public int selectedLevel;

    [Header("Starting Match Menu")]
    [SerializeField] GameObject buttonSetParent;
    [SerializeField] GameObject[] playerOptionsParents;
    [SerializeField] GameObject[] robots;

    void Start() {
        //Fetch the Dropdown GameObject the script is attached to
        // button_set = GetComponent<TMPro.TMP_Dropdown>();
        
        Button[] levelButtonSet = buttonSetParent.GetComponentsInChildren<Button>();

        // List of level names
        // TODO: This is not extensible right now. Fix it.
        for (int i = 0; i < mds.levels.Length; i++) {
            Level currLevel = mds.levels[i].GetComponent<Level>();
            levelButtonSet[i].GetComponent<Image>().sprite = currLevel.thumbnail;
            levelButtonSet[i].gameObject.GetComponentInChildren<TMP_Text>().text = currLevel.nid;
            levelButtonSet[i].gameObject.GetComponent<MatchMenuSelector>().buttonOptionNumber = i;
        }
        mds.levelIdx = 0;
        levelButtonSet[0].interactable = false;

        for (int i = 0; i < mds.numPlayers; i++) { // Two for the players, two for the options (color and secondary type)
            Button[] optionsButtons = playerOptionsParents[i].GetComponentsInChildren<Button>();
            Color temp = mds.primaryColours[mds.playerColourSchemes[i]];
            temp.a = 1f;
            optionsButtons[0].GetComponent<Image>().color = temp;
            
            optionsButtons[1].GetComponent<Image>().sprite = mds.secondaryTypes[mds.playerSecondaries[i]].GetComponent<BulletLogic>().thumbnail;
            optionsButtons[1].gameObject.GetComponentInChildren<TMP_Text>().text = 
                mds.secondaryTypes[mds.playerSecondaries[i]].GetComponent<BulletLogic>().label;
            print(mds.secondaryTypes[mds.playerSecondaries[i]].GetComponent<BulletLogic>().label);
            
            int playerIndex = mds.playerColourSchemes[i];
            var colourizer = robots[i].GetComponent<PlayerColourizer>();
            colourizer.PrimaryColour = mds.primaryColours[playerIndex];
            colourizer.SecondaryColour = mds.accentColours[playerIndex];
            colourizer.initialColourize();
        }
    }

    public override void ColourUpdate(int player) {
        int playerIndex = mds.playerColourSchemes[player];
        var colourizer = robots[player].GetComponent<PlayerColourizer>();
        colourizer.PrimaryColour = mds.primaryColours[playerIndex];
        colourizer.SecondaryColour = mds.accentColours[playerIndex];
    }
    
    public override void ChooseLevel(int levelNumber) {
        selectedLevel = levelNumber;
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
}
