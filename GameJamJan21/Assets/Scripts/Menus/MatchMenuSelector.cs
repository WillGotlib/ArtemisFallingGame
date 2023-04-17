using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchMenuSelector : MonoBehaviour
{
    [SerializeField] private MatchSetupMenu matchMenu;
    // [SerializeField] private MidMatchMenu midMatchMenu;
    
    public int buttonOptionNumber;
    public int playerNumber; // -1 is not associated with a player
    public MatchDataScriptable mds;

    private bool selectionOnCooldown = false;
    
    public void ChooseMap() {
        // print(buttonOptionNumber.ToString());
        matchMenu.ChooseLevel(buttonOptionNumber);
        // Selectable next = GetComponent<Button>().FindSelectable(new Vector3(1,0,0));
        // if (!next) { next = GetComponent<Button>().FindSelectable(new Vector3(-1,0,0));
        // }
        // next.Select();

        matchMenu.HighlightButton(GetComponent<Button>());
        // GetComponent<Button>().interactable = false;
    }

    // Note: This function isn't going to be used in the final build
    // since we're depricating colour switching.
    // public void NextColor() {
    //     if (selectionOnCooldown) { return; }
    //     selectionOnCooldown = true;
        
    //     int speculativeNext = (mds.playerColourSchemes[playerNumber] + 1) % mds.primaryColours.Length;
        
    //     string a = "SELECTED: (";
    //     for (int i = 0; i < mds.selectedColourSchemes.Count; i++) {
    //         a  = a + mds.selectedColourSchemes[i] + ", ";
    //     }
    //     print(a + ")");

    //     for (int i = 0; i < mds.primaryColours.Length; i++) 
    //     {   // Will stop if the colour scheme is free, or none are
    //         print("CYCLING: " + speculativeNext);
    //         if (!mds.selectedColourSchemes.Contains(speculativeNext)) { break; } // This one is free.
    //         speculativeNext = (speculativeNext + 1) % mds.primaryColours.Length;
    //     }
    //     print("LANDED ON: " + speculativeNext);
    //     if (!mds.selectedColourSchemes.Contains(speculativeNext)) { // We're switching colours
    //         print("PREVIOUSLY SELECTED: " + mds.playerColourSchemes[playerNumber]);
    //         mds.selectedColourSchemes.Remove(mds.playerColourSchemes[playerNumber]);
    //         mds.selectedColourSchemes.Add(speculativeNext);
    //         buttonOptionNumber = speculativeNext;
    //         mds.playerColourSchemes[playerNumber] = speculativeNext;
    //     }
    //     Color temp = mds.primaryColours[speculativeNext];
    //     temp.a = 1f;
    //     GetComponent<Image>().color = temp;
        
    //     matchMenu.ColourUpdate(playerNumber);
    //     StartCoroutine(SelectCooldown());
    // }

    // Note: This function won't be used either because we switched around how secondaries work.
    public void NextSecondary() {
        if (selectionOnCooldown) { return; }
        
        selectionOnCooldown = true;
        mds.playerSecondaries[playerNumber] = (mds.playerSecondaries[playerNumber] + 1) % mds.secondaryTypes.Length;
        buttonOptionNumber = mds.playerSecondaries[playerNumber];
        GetComponent<Image>().sprite = mds.secondaryTypes[buttonOptionNumber].GetComponent<BulletLogic>().thumbnail;
        GetComponentInChildren<TMP_Text>().text = mds.secondaryTypes[buttonOptionNumber].GetComponent<BulletLogic>().label;
        StartCoroutine(SelectCooldown());
    }

    private IEnumerator SelectCooldown()
    {
        yield return null;
        selectionOnCooldown = false;
    }
}
