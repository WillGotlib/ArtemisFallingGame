using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class multiScreenItem : MonoBehaviour
{
    public string sessionName = "SessionId";
    public int maxPlayers = 4;
    public int playerAmount = 0;

    public delegate void SelectedCallback(string sessionID);

    public SelectedCallback selectFunction;

    //public TMP_Text playerAmountObject;
    // public TMP_Text maxPlayerObject;
    // public TMP_Text sessionNameObject;
    void Start()
    {
        GetTextFromChild(0).text = playerAmount.ToString();
        GetTextFromChild(1).text = maxPlayers.ToString();
        GetTextFromChild(3).text = sessionName;
    }

    private TMP_Text GetTextFromChild(int index)
    {
        return transform.GetChild(index).GetComponent<TMP_Text>();
    }
    
    
    public void _click()
    {
        selectFunction(sessionName);
    }
}
