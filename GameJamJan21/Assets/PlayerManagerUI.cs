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

    private List<MenuCursor> playerCursors = new List<MenuCursor>();
    private List<MultiplayerEventSystem> playerES = new List<MultiplayerEventSystem>();
    
    // Update is called once per frame
    void Start()
    {
        print("Starting");
        // Instantiate(PlayerPrefab);
        // OnPlayerJoined(); // Call this once
    }

    public void RefreshCursors(GameObject next) {
        for (int i = 0; i < playerCursors.Count; i++) {
            print("player " + playerCursors[i] + " selected " + next);
            playerES[i].SetSelectedGameObject(next);
            // playerCursors[i].refresh(Vector2.zero);
            playerCursors[i].MoveToTarget(next, new Vector3(0, 0, 0));
        }
    }

    public void SelectionCheck() {
        // Make sure that if there's a cursor beside a Selectable, it's selected
        for (int i = 0; i < playerCursors.Count; i++) {
            print("CURRENTLY SELECTED: " + playerES[i].currentSelectedGameObject);
            // playerES[i].currentSelectedGameObject.GetComponent<Selectable>().Select();
            
            // GameObject curr = playerCursors[i].currentlySelected;
            // curr.GetComponent<Selectable>().Select();
        }
    }

    void OnPlayerJoined() {
        print("Player Joined");
        GameObject newPlayer = Instantiate(MPEventSystem, transform);
        MultiplayerEventSystem playerEventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
        playerEventSys.firstSelectedGameObject = DefaultFirstSelected[currNumPlayers];
        playerEventSys.playerRoot = canvas;
        playerEventSys.SetSelectedGameObject(DefaultFirstSelected[currNumPlayers]);
        playerES.Add(playerEventSys);

        currNumPlayers++;
        newPlayer.name = "MenuEventSystem P" + currNumPlayers;

        GameObject go = GameObject.Find ("MenuPlayer(Clone)"); // Temp
        
        //if the character doesn't exist we need to manually spawn them in.
        if (!go) {
            GameObject menuPlayer = Instantiate(PlayerPrefab, canvas.transform);
        }
        go.name = "MenuP" + currNumPlayers;
        go.GetComponent<PlayerInput>().uiInputModule = GetComponent<InputSystemUIInputModule>();
        
        MenuCursor x = go.GetComponent<MenuCursor>();
        x.playerNumber = currNumPlayers - 1;
        x.LoadCursorImage(currNumPlayers - 1);
        print("GETTING EVENT SYSTEM: " + newPlayer.GetComponent<EventSystem>());
        x._eventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
        print("NEW ROOT: " + newPlayer.GetComponent<MultiplayerEventSystem>());
        go.transform.SetParent(canvas.transform);
        x.setManager(this);
        playerCursors.Add(x);
        Debug.Log(go.name + " added.");
        x.refresh(Vector2.zero);
        // print(go);
    }

    void Awake() {
        reset();
        for (int i = 0; i < currNumPlayers; i++) {
            OnPlayerJoined();
        }
    }

    void reset() {
        currNumPlayers = 0;
    }
}
