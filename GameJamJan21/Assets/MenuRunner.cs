using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRunner : MonoBehaviour
{
    public int startingIndex;

    [Header("Parallel arrays. Must be same length.")]
    public GameObject[] menusPriority;
    public Button[] defaultButtons;

    public MainMenu MainMenu;
    public LevelSelectMenu LevelSelectMenu;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject menu in menusPriority) menu.SetActive(false);
        menusPriority[startingIndex].SetActive(true);
        LevelSelectMenu.RefreshPlayerSections();
    }

    public Button GetCurrentMenuDefault() {
        for (int i = 0; i < menusPriority.Length; i++) {
            if (menusPriority[i].activeSelf) return defaultButtons[i];
        }
        // It should never get here. Exactly one menu should be active at all times.
        return null;
    }

    public void RefreshPlayerSections() {
        LevelSelectMenu.RefreshPlayerSections();
    }
}
