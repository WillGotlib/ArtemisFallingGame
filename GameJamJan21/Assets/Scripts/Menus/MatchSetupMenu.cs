using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchSetupMenu : MonoBehaviour
{
    [Header("Generic Match Setup Menu")]
    public MatchDataScriptable mds;
    public int selectedLevel;
    
    public abstract void PlayGame();

    public abstract void ChooseLevel(int levelNumber);

    public abstract void ColourUpdate(int playerNumber);
}
