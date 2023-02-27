using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MatchDataScriptable", order = 1)]
public class MatchDataScriptable : ScriptableObject
{
    public int levelIdx;
    public int numGames;
}
