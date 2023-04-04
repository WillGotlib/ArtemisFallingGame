using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MatchDataScriptable", order = 1)]
public class MatchDataScriptable : ScriptableObject
{
    public int numPlayers;

    public int levelIdx;
    public int numGames;
    [SerializeField] public GameObject[] levels = {};
    public int[] playerWins;
    public int lastWinner;

    [Header("Colours")]
    [Tooltip("Lists have to be the same length")]
    [SerializeField] public Color[] primaryColours;
    [SerializeField] public Color[] accentColours;

    public GameObject[] secondaryTypes;
    public int[] playerSecondaries;

    public bool skipMainMenu = false;
    
    
    private int[] playerColourSchemes;
    private List<int> selectedColourSchemes; // For no overlap
}