using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject settingUI;
    // Update is called once per frame

    public void SwitchMenuState() {
        settingUI.SetActive(false);
        print("is paused: " + isPaused);
        if (isPaused == true) {
            Resume();
        }
        else {
            Pause();
        }
    }

    void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame() {
        Resume();
    }

    public void ReturnToMenu() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            Destroy(player);
        }
        SceneManager.LoadScene("Menu2");
    }

    public void QuitGame() {
        MainMenu.QuitGame();
    }
}
