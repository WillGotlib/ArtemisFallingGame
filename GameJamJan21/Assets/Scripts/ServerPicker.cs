using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerPicker : MonoBehaviour
{
    public GameObject elementPrefab;
    public Transform elementContainer;

    public TMP_InputField serverAddr;
    public TMP_InputField sessionName;
    
    void Start()
    {
        var maxP = Random.Range(4, 7);
        //var container = transform.GetChild(0).GetChild(0).GetChild(0);
        for (int i = 0; i < 8; i++)
        {
            var o = Instantiate(elementPrefab,elementContainer);
            var script = o.GetComponent<multiScreenItem>();
            script.sessionName = "session #" + i;
            script.maxPlayers = maxP;
            script.playerAmount = Random.Range(0, maxP+1);
            script.selectFunction = UpdateSelection;
        }
    }

    private void UpdateSelection(string s)
    {
        sessionName.SetTextWithoutNotify(s);
        Debug.Log("selected session: " + s);
    }

}