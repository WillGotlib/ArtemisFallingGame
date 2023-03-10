using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MidMatchMenu : MonoBehaviour
{
    public TMP_Text p1WinText;
    public TMP_Text p2WinText;
    public MatchDataScriptable matchDataScriptable;
    public int selectedLevel;

    [SerializeField] GameObject buttonSetParent;
    int m_Index;

    public void Start() {
        p1WinText.text = ""+matchDataScriptable.p1Wins;
        p2WinText.text = ""+matchDataScriptable.p2Wins;

        Button[] buttonSet = buttonSetParent.GetComponentsInChildren<Button>();

        // List of level names
        // TODO: This is not extensible right now. Fix it.
        for (int i = 0; i < matchDataScriptable.levels.Length; i++) {
            Level currLevel = matchDataScriptable.levels[i].GetComponent<Level>();
            buttonSet[i].GetComponent<Image>().sprite = currLevel.thumbnail;
            buttonSet[i].gameObject.GetComponentInChildren<TMP_Text>().text = currLevel.nid;
            buttonSet[i].gameObject.GetComponent<MatchLevelSelector>().buttonOptionNumber = i;
        }
        matchDataScriptable.levelIdx = 0;
    }

    public void ChooseLevel(int levelNumber) {
        selectedLevel = levelNumber;
        matchDataScriptable.levelIdx = levelNumber;
        print("Selected " + levelNumber);
    }

    public void PlayGame() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }

        SceneManager.LoadScene("Gameplay");
    }
}
