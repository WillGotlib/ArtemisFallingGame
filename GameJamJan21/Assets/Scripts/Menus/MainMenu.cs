using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text version;
    public void Start()
    {
        version.text = Application.version;
    }

    public void QuitGame() {
        Debug.Log("Good Bye");
        Application.Quit();
    }
}