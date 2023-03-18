using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MidMatchMenu : MatchSetupMenu
{
    [Header("Mid-Match Menu")]
    // These fields COULD be abstracted to the SetupMenu class
    [SerializeField] GameObject buttonSetParent;
    [SerializeField] GameObject[] robots; // for colourization

    [SerializeField] RawImage[] robotCams;

    [Header("Text elements")]
    public TMP_Text p1WinText;
    public TMP_Text p2WinText;
    [SerializeField] TMP_Text currentWinnerText;
    int m_Index;

    public void Start() {
        p1WinText.text = ""+ mds.p1Wins;
        p2WinText.text = ""+ mds.p2Wins;
        currentWinnerText.text = $"PLAYER {mds.lastWinner} WINS!";
        for (int i = 0; i < mds.numPlayers; i++) {
            if (i == mds.lastWinner) robotCams[i].color = Color.white;
            else robotCams[i].color = Color.grey;
        }

        Button[] buttonSet = buttonSetParent.GetComponentsInChildren<Button>();

        // List of level names
        // TODO: This is not extensible right now. Fix it.
        for (int i = 0; i < mds.levels.Length; i++) {
            Level currLevel = mds.levels[i].GetComponent<Level>();
            buttonSet[i].GetComponent<Image>().sprite = currLevel.thumbnail;
            buttonSet[i].gameObject.GetComponentInChildren<TMP_Text>().text = currLevel.nid;
            buttonSet[i].gameObject.GetComponent<MatchMenuSelector>().buttonOptionNumber = i;
        }
        mds.levelIdx = 0;

        // Copied code from MatchMenu. Should be abstracted.
        for (int i = 0; i < mds.numPlayers; i++) { // Two for the players, two for the options (color and secondary type)
            ColourUpdate(i);
        }
    }

    // This should really only be executed once on this menu per player,
    // upon load to make sure the robots look right.
    public override void ColourUpdate(int playerNumber) {
        int playerIndex = mds.playerColourSchemes[playerNumber];
            var colourizer = robots[playerNumber].GetComponent<PlayerColourizer>();
            colourizer.PrimaryColour = mds.primaryColours[playerIndex];
            colourizer.SecondaryColour = mds.accentColours[playerIndex];
            colourizer.initialColourize();
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
