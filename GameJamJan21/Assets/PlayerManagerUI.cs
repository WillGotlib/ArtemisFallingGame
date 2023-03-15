using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerManagerUI : MonoBehaviour
{
    [SerializeField] GameObject MPEventSystem;
    [SerializeField] GameObject[] DefaultFirstSelected;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject PlayerPrefab;

    public static int currNumPlayers = 0;

    private List<MenuCursor> players = new List<MenuCursor>();

    // Update is called once per frame
    void Start()
    {
        print("Starting");
        // Instantiate(PlayerPrefab);
        // OnPlayerJoined(); // Call this once
    }

    public void RefreshCursors(GameObject next) {
        foreach (MenuCursor player in players) {
            print("player " + player + " selected " + next);
            player.gameObject.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(next);
            player.OnNavigate();
            print(player.gameObject.transform.position);
            
        }
    }

    void OnPlayerJoined() {
        print("Player Joined");
        GameObject newPlayer = Instantiate(MPEventSystem, transform);
        MultiplayerEventSystem playerEventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
        playerEventSys.SetSelectedGameObject(DefaultFirstSelected[currNumPlayers]);
        currNumPlayers++;
        newPlayer.name = "MenuEventSystem P" + currNumPlayers;

        GameObject go = GameObject.Find ("MenuPlayer(Clone)"); // Temp
        
        //if the tree exist then destroy it
        if (go) {
            // Destroy (go.gameObject);
            go.name = "MenuP" + currNumPlayers;
            go.GetComponent<PlayerInput>().uiInputModule = GetComponent<InputSystemUIInputModule>();
            MenuCursor x = go.GetComponent<MenuCursor>();
            x.LoadCursorImage(currNumPlayers - 1);
            print("GETTING EVENT SYSTEM: " + newPlayer.GetComponent<EventSystem>());
            x._eventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
            print("NEW ROOT: " + newPlayer.GetComponent<MultiplayerEventSystem>());
            go.transform.SetParent(canvas.transform);
            print(x);
            players.Add(x);
            Debug.Log(go.name + " added."); 
            x.OnNavigate();
        }
        // print(go);
    }
}
