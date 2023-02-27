using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MidMatchMenu : MonoBehaviour
{
    public TMP_Text p1WinText;
    public TMP_Text p2WinText;
    public MatchDataScriptable matchDataScriptable;

    public void Start() {
        p1WinText.text = ""+matchDataScriptable.p1Wins;
        p2WinText.text = ""+matchDataScriptable.p2Wins;
    }

    public void PlayGame() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }

        SceneManager.LoadScene("Gameplay");
    }
}
