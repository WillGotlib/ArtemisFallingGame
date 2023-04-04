using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text version;
    public MatchDataScriptable mds;

    public void Start()
    {
        ResetLanding();
        version.text = Application.version;
    }

    [SerializeField] GameObject PreMatchMenu;
    [SerializeField] Button PreMatchButton;


    public void Awake() {
        if (mds.skipMainMenu) {
            mds.skipMainMenu = false;
            PreMatchButton.onClick.Invoke();
        }
    }

    public void PlayGame() {
        if (mds.numPlayers < 2) { 
            print("ERROR: Tried to play game with under min # of players (2)");
            return; 
        }
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayTutorial() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }

        SceneManager.LoadScene("Tutorial");
    }

    public void ResetLanding()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<Controller>().HideGun();
            player.GetComponentInChildren<AnimationUtils>().Landing = false;
        }
    }

    public void ShowPrematchMenu() 
    { 
        PreMatchMenu.GetComponent<LevelSelectMenu>().CueRobotDrop();
        print("Showing prematch menu!");
    }

    public static void QuitGame() {
        Debug.Log("Good Bye");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}