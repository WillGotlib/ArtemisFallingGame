using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MatchLevelSelector : MonoBehaviour
{
    [SerializeField] private MatchMenu matchMenu;
    [SerializeField] private MidMatchMenu midMatchMenu;
    
    public int buttonOptionNumber;
    public MatchDataScriptable matchDataScriptable;
    
    public void ChooseMap() {
        print(buttonOptionNumber.ToString());
        // this.GetComponent<Button>().Select();
        // matchDataScriptable.levelIdx = buttonOptionNumber;
        if (midMatchMenu == null) {
            matchMenu.ChooseLevel(buttonOptionNumber);
        } else {
            midMatchMenu.ChooseLevel(buttonOptionNumber);
        }
        
    }
}
