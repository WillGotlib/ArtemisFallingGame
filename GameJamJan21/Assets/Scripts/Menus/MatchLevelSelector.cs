using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MatchLevelSelector : MonoBehaviour
{
    [SerializeField] private MatchMenu MatchMenu;
    
    public int buttonOptionNumber;
    public MatchDataScriptable matchDataScriptable;
    
    public void ChooseMap() {
        print(buttonOptionNumber.ToString());
        // this.GetComponent<Button>().Select();
        // matchDataScriptable.levelIdx = buttonOptionNumber;
        MatchMenu.ChooseLevel(buttonOptionNumber);
    }
}
