using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrematchDropdown : MonoBehaviour
{
    TMPro.TMP_Dropdown m_Dropdown;
    int m_Index;
    public MatchDataScriptable matchDataScriptable;

    void Start() {
        //Fetch the Dropdown GameObject the script is attached to
        m_Dropdown = GetComponent<TMPro.TMP_Dropdown>();
        //Clear the old options of the Dropdown menu
        m_Dropdown.ClearOptions();
        // List of level names
        List<TMPro.TMP_Dropdown.OptionData> m_Names = new List<TMPro.TMP_Dropdown.OptionData>();

        foreach (GameObject level in matchDataScriptable.levels) {
            TMPro.TMP_Dropdown.OptionData newLevel = new TMPro.TMP_Dropdown.OptionData();
            newLevel.text = level.GetComponent<Level>().nid;
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
        matchDataScriptable.levelIdx = m_Index;
        m_Dropdown.onValueChanged.AddListener(delegate { ChangeSelectedLevel(); });
    }

    void ChangeSelectedLevel() {
        matchDataScriptable.levelIdx = m_Dropdown.value;
    }
}
