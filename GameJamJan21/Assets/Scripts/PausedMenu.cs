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
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Debug.Log("Game exited!");
        Application.Quit();
    }
}
