using System;
using Google.Protobuf.Collections;
using Grpc.Core;
using Online;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Server = protoBuff.Server;

public class ServerPicker : MonoBehaviour
{
    public string gameSceneName;
    
    [Header("Work values")]
    public GameObject elementPrefab;
    public Transform elementContainer;

    public Button joinButton;

    public GameObject serverChooser;
    public TMP_InputField serverAddr;
    public TMP_InputField sessionName;

    private RepeatedField<Server> servers;
    
    void Start()
    {
        serverAddr.SetTextWithoutNotify(Connection.GetAddress());
        serverChooser.SetActive(false);
        if (gameSceneName == "")
        {
            throw new Exception("scene is required");
        }

        UpdateServer();
    }

    public void ServerValid()
    {
        var input = sessionName.text;
        
        foreach (var server in servers)
        {
            if (input == server.Id && server.Max == server.Online)
            {
                joinButton.interactable = false;
                return;
            }
        }
        joinButton.interactable = true;
    }

    public void ConnectToSession()
    {
        var sessionID = sessionName.text;
        if (sessionID.Length < 3 || sessionID.Length > 25)
        {
            Debug.Log("bad session ID"); //todo change colour or something
            return;
        }

        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single); //todo loading bar

        SceneManager.sceneLoaded += GetLoadFunc(sessionID);
    }
    private static UnityAction<Scene,LoadSceneMode> _sceneChangeFunc;

    private UnityAction<Scene, LoadSceneMode> GetLoadFunc(string sessionID)
    {
        async void Func(Scene scene, LoadSceneMode sceneMode)
        {
            Debug.Log("onSceneChange");
            if (!await FindObjectOfType<NetworkManager>().Connect(sessionID))
                SceneManager.LoadScene("ServerSelector",
                    LoadSceneMode.Single); // return to selector scene if load failed
            SceneManager.sceneLoaded += _sceneChangeFunc;
        }
        
        _sceneChangeFunc = Func;
        return Func;
    }
    
    public async void UpdateServer()
    {
        serverChooser.SetActive(false);
        GRPC.Dispose();
        var channel = await Connection.ChangeAddress(serverAddr.text);
        if (channel.State != ChannelState.Ready && channel.State != ChannelState.Idle)
        {
            Debug.Log("No channel, channel is " + channel.State);
            return;
        }
        serverChooser.SetActive(true);
        
        GetServers();
    }

    private async void GetServers()
    {
        foreach (Transform child in elementContainer.transform)
            Destroy(child.gameObject);
        
        servers = await GRPC.List();
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