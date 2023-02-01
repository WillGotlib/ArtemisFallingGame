using System;
using Grpc.Core;
using Online;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ServerPicker : MonoBehaviour
{
    public string gameSceneName;
    
    [Header("Work values")]
    public GameObject elementPrefab;
    public Transform elementContainer;

    public GameObject serverChooser;
    public TMP_InputField serverAddr;
    public TMP_InputField sessionName;

    void Start()
    {
        serverAddr.SetTextWithoutNotify(Connection.GetAddress());
        serverChooser.SetActive(false);
        if (gameSceneName == "")
        {
            throw new Exception("scene is required");
        }
    }

    public void ConnectToSession()
    {
        var sessionID = sessionName.text;
        if (sessionID.Length < 3 || sessionID.Length > 25)
        {
            Debug.Log("bad session ID"); //todo change colour or something
            return;
        }

        if (FindObjectOfType<NetworkManager>().Connect(sessionID))
            SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single); //todo loading bar
    }

    public void UpdateServer()
    {
        var channel = Connection.ChangeAddress(serverAddr.text);
        if (channel.State != ChannelState.Ready && channel.State != ChannelState.Idle)
        {
            serverChooser.SetActive(false);
            Debug.Log("No channel, channel is " + channel.State);
            return;
        }
        serverChooser.SetActive(true);
        
        GetServers();
    }

    private void GetServers()
    {
        foreach (Transform child in elementContainer.transform)
            Destroy(child.gameObject);
        
        var servers = GRPC.List();
        if (servers.Count == 0)
        {
            UpdateSelection("game " + Random.Range(0, 2057));
            return;
        }

        for (var i = 0; i < servers.Count; i++)
        {
            var server = servers[i];
            if (i == 0)
            {
                UpdateSelection(server.Id);
            }

            var o = Instantiate(elementPrefab, elementContainer);
            var script = o.GetComponent<multiScreenItem>();
            script.sessionName = server.Id;
            script.maxPlayers = (int)server.Max;
            script.playerAmount = (int)server.Online;
            script.selectFunction = UpdateSelection;
        }
    }

    private void UpdateSelection(string s)
    {
        sessionName.SetTextWithoutNotify(s);
        Debug.Log("selected session: " + s);
    }
}