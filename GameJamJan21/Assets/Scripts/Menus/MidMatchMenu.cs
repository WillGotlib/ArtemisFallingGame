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

    [SerializeField] RawImage[] robotCams;

    [Header("Text elements")]
    public TMP_Text p1WinText;
    public TMP_Text p2WinText;
    [SerializeField] TMP_Text currentWinnerText;
    int m_Index;

    public void Start() {
        p1WinText.text = ""+ mds.playerWins[0];
        p2WinText.text = ""+ mds.playerWins[1];
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

        initialColors();
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
}
