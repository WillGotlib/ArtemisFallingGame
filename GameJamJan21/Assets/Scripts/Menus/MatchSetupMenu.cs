using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MatchSetupMenu : MonoBehaviour
{
    [Header("Generic Match Setup Menu")]
    public MatchDataScriptable mds;
    public GameObject[] robots; // for colourization
    
    public abstract void PlayGame();

    public abstract void ChooseLevel(int levelNumber);

    public void initialColors() {
        // int numPlayers = mds.numPlayers;
        int numPlayers = 4; // Might as well.
        for (int i = 0; i < numPlayers; i++) {
            ColourUpdate(i);
        }
    }

    public void ColourUpdate(int playerNumber) {
        ColourUpdate(playerNumber, robots[playerNumber]);
    }

    public void ColourUpdate(int playerNumber, GameObject target) {
        // int playerIndex = mds.playerColourSchemes[playerNumber];
        int playerIndex = playerNumber;
        print("Colour player " + playerNumber + ", " + mds.primaryColours[0]);
        var colourizer = target.GetComponent<PlayerColourizer>();
        print("Colourizer: " + colourizer);
        colourizer.PrimaryColour = mds.primaryColours[playerIndex];
        colourizer.SecondaryColour = mds.accentColours[playerIndex];
        colourizer.initialColourize();
    }

    public void HighlightButton(Button b) {
        var colors = b.colors;
        colors.normalColor = Color.green;
        b.colors = colors;
    }

    public void UnHighlightButton(Button b) {
        var colors = b.colors;
        colors.normalColor = Color.white;
        b.colors = colors;
    }
}
