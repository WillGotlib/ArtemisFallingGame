using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchSetupMenu : MonoBehaviour
{
    [Header("Generic Match Setup Menu")]
    public MatchDataScriptable mds;
    public GameObject[] robots; // for colourization
    
    public abstract void PlayGame();

    public abstract void ChooseLevel(int levelNumber);

    public void initialColors() {
        for (int i = 0; i < mds.numPlayers; i++) {
            ColourUpdate(i);
        }
    }

    public void ColourUpdate(int playerNumber) {
        ColourUpdate(playerNumber, robots[playerNumber]);
    }

    public void ColourUpdate(int playerNumber, GameObject target) {
        int playerIndex = mds.playerColourSchemes[playerNumber];
        var colourizer = target.GetComponent<PlayerColourizer>();
        colourizer.PrimaryColour = mds.primaryColours[playerIndex];
        colourizer.SecondaryColour = mds.accentColours[playerIndex];
        colourizer.initialColourize();
    }
}
