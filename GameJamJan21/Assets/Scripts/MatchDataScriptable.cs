using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MatchDataScriptable", order = 1)]
public class MatchDataScriptable : ScriptableObject
{
    public int levelIdx;
    public int numGames;
    [SerializeField] public GameObject[] levels = {};
    public int p1Wins;
    public int p2Wins;
}
