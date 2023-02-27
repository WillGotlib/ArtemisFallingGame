using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MatchLevelSelector : MonoBehaviour
{
    [SerializeField] private MatchMenu MatchMenu;

    public Button thisButton;
    
    public int levelNumber;
    
    public void ChooseMap() {
        print(levelNumber.ToString());
        thisButton.Select();
        MatchMenu.ChooseLevel(levelNumber - 1);
    }
}
