using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NumGameSelectDropdown : MonoBehaviour
{
    TMPro.TMP_Dropdown m_Dropdown;
    int m_Index;
    public MatchDataScriptable matchDataScriptable;
    public string[] rounds = {};

    void Start() {
        //Fetch the Dropdown GameObject the script is attached to
        m_Dropdown = GetComponent<TMPro.TMP_Dropdown>();
        //Clear the old options of the Dropdown menu
        m_Dropdown.ClearOptions();
        // List of num rounds
        List<TMPro.TMP_Dropdown.OptionData> m_Names = new List<TMPro.TMP_Dropdown.OptionData>();

        foreach (string option in rounds) {
            TMPro.TMP_Dropdown.OptionData newLevel = new TMPro.TMP_Dropdown.OptionData();
            newLevel.text = option;
            m_Names.Add(newLevel);
        }

        //Take each entry in the message List
        foreach (TMPro.TMP_Dropdown.OptionData message in m_Names)
        {
            //Add each entry to the Dropdown
            m_Dropdown.options.Add(message);
            //Make the index equal to the total number of entries
            m_Index = m_Names.Count - 1;
        }

        m_Dropdown.value = m_Index;
        matchDataScriptable.numGames = int.Parse(m_Dropdown.options[m_Index].text);
        m_Dropdown.onValueChanged.AddListener(delegate { ChangeNumGames(); });
    }

    void ChangeNumGames() {
        matchDataScriptable.numGames = int.Parse(m_Dropdown.options[m_Dropdown.value].text);
    }
}