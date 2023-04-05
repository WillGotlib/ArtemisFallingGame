using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectMenu : MatchSetupMenu
{    
    [Header("Starting Level Select Menu")]
    [SerializeField] Button PlayButton;
    [SerializeField] GameObject levelButtonParent;
    [SerializeField] GameObject gamesButtonParent;

    [SerializeField] GameObject[] playerOptionsSections;

    [SerializeField] MenuRunner MenuRunner;

    void Start() {
        //Fetch the Dropdown GameObject the script is attached to
        // button_set = GetComponent<TMPro.TMP_Dropdown>();
        
        Button[] levelButtonSet = levelButtonParent.GetComponentsInChildren<Button>();

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

        for (int i = 0; i < mds.numPlayers; i++) {
            print("Dropping robot " + i);
            CueRobotDrop(i);
        }

        initialColors();
        RefreshPlayerSections();
    }

    public void CueRobotDrop() {
        for (int i = 0; i < mds.numPlayers; i++) {
            robots[i].GetComponentInChildren<AnimationUtils>().PlayLanding();
        }
    }

    void CueRobotDrop(int playerNumber) {
        robots[playerNumber].GetComponentInChildren<AnimationUtils>().PlayLanding();
    }

    public void ResetRobotDrops() {
        for (int i = 0; i < mds.numPlayers; i++) {
            robots[i].GetComponent<Controller>().HideGun();
            robots[i].GetComponentInChildren<AnimationUtils>().Landing = false;
        }
    }
    
    public override void ChooseLevel(int levelNumber) {
        mds.levelIdx = levelNumber;
        print("Selected " + levelNumber);

        // Find the disabled level and bring it back
        Button[] levelButtonSet = levelButtonParent.GetComponentsInChildren<Button>();
        foreach (Button level in levelButtonSet) { level.interactable = true; }
    }

    public void ChooseNumGames(int numGames) {
        Button[] gamesButtonSet = gamesButtonParent.GetComponentsInChildren<Button>();
        foreach (Button level in gamesButtonSet) { level.interactable = true; }
    }

    public void RefreshPlayerSections() {
        print("Number of players is " + mds.numPlayers);
        for (int i = 0; i < 4; i++) {
            if (i >= mds.numPlayers) playerOptionsSections[i].SetActive(false);
            else if (!playerOptionsSections[i].activeSelf) {
                print("activating a new section");
                playerOptionsSections[i].SetActive(true);
                CueRobotDrop(i);
            }
        }

        if (mds.numPlayers < 2) { 
            var c = PlayButton.colors;
            c.normalColor = Color.gray; 
            PlayButton.colors = c;
        }
        else {
            var c = PlayButton.colors;
            c.normalColor = Color.clear;
            PlayButton.colors = c;
        }
        if (!PlayButton.interactable && mds.numPlayers >= 2) {
            PlayButton.interactable = true;
        }
    }

    public override void PlayGame() {
        if (mds.numPlayers < 2) { 
            print("ERROR: Tried to play game with under min # of players (2)");
            return; 
        }
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }
        if (MenuRunner.navigationCooldownComplete) SceneManager.LoadScene("Gameplay");
        else print("Hit play button too fast. Didn't load into scene");
    }
}
