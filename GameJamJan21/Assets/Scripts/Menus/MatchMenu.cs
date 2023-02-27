using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchMenu : MonoBehaviour
{    
    public int selectedLevel;
    
    public void ChooseLevel(int levelNumber) {
        selectedLevel = levelNumber;
        print("Selected " + levelNumber);
    }

    public void PlayGame() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }
        Statics.setLevel(selectedLevel);
        print("SELECTED: " + selectedLevel + " STATIC: " + Statics.selectedLevel);
        SceneManager.LoadScene("Gameplay");
    }
}
