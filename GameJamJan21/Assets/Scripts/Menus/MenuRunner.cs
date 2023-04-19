using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRunner : MonoBehaviour
{
    [NonSerialized] public int startingIndex;
    public int currentIndex;

    [Header("Parallel arrays. Must be same length.")]
    public GameObject[] menusPriority;
    public Button[] defaultButtons;

    public MainMenu MainMenu;
    public LevelSelectMenu LevelSelectMenu;
    [SerializeField] PlayerManagerUI PlayerManagerUI;

    public bool navigationCooldownComplete = true;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject menu in menusPriority) menu.SetActive(false);
        menusPriority[startingIndex].SetActive(true);
        LevelSelectMenu.RefreshPlayerSections();
        navigationCooldownComplete = true;
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

    private IEnumerator NavigationCooldown() {
        yield return null;
        navigationCooldownComplete = true;
    }

    public void SwitchToMenu(int newIndex) {
        if (newIndex > menusPriority.Length) return;
        print($"newIndex: {newIndex}, oldIndex: {currentIndex}");
        if (!navigationCooldownComplete) {
            print("Tried to navigate between menus too fast. Chill the fuck out dude");
            return;
        }
        navigationCooldownComplete = false;
        StartCoroutine(NavigationCooldown());

        menusPriority[currentIndex].SetActive(false);
        menusPriority[newIndex].SetActive(true);
        PlayerManagerUI.RefreshCursors(defaultButtons[newIndex].gameObject);

        currentIndex = newIndex;
        
    }
}
