using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerManagerUI : MonoBehaviour
{
    public MatchDataScriptable mds;
    
    [SerializeField] GameObject MPEventSystem;
    [SerializeField] MenuRunner MenuRunner;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] CurrentControlScheme controlScheme;

    public static int currNumPlayers = 0;

    private List<MenuCursor> playerCursors = new List<MenuCursor>();
    private List<MultiplayerEventSystem> playerES = new List<MultiplayerEventSystem>();
    private GameObject[] previousTargets;
    
    // Update is called once per frame
    void Start()
    {
        mds.numPlayers = currNumPlayers;
        previousTargets = new GameObject[mds.maxPlayers];
        //print("Starting");
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
        for (int i = 0; i < playerCursors.Count; i++) {
            SelectionCheck(i);
        }
    }

    public void SelectionCheck(int playerNumber) {
        // Make sure that if there's a cursor beside a Selectable, it's selected
        // Also indicates that THIS IS THE PLAYER MOVING RIGHT NOW.
        print("[P" + playerNumber + "] CURRENTLY SELECTED: " + playerES[playerNumber].currentSelectedGameObject);
        playerCursors[playerNumber].MoveToTarget(playerES[playerNumber].currentSelectedGameObject, Vector3.zero);
        
        previousTargets[playerNumber] = playerES[playerNumber].currentSelectedGameObject;
        for (int i = 0; i < playerES.Count; i++) {
            playerES[i].SetSelectedGameObject(previousTargets[i]);
        }
    }

    public Button GetCurrentMenuDefault() {
        return MenuRunner.GetCurrentMenuDefault();
    }

    void OnPlayerJoined() {
        print("Player Joined");
        GameObject newPlayer = Instantiate(MPEventSystem, transform);
        MultiplayerEventSystem playerEventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
        playerEventSys.firstSelectedGameObject = GetCurrentMenuDefault().gameObject;
        playerEventSys.playerRoot = canvas;
        playerEventSys.SetSelectedGameObject(playerEventSys.firstSelectedGameObject);
        playerES.Add(playerEventSys);

        previousTargets[currNumPlayers] = playerEventSys.firstSelectedGameObject;

        currNumPlayers++;
        newPlayer.name = "MenuEventSystem P" + currNumPlayers;

        GameObject go = GameObject.Find ("MenuPlayer(Clone)"); // Temp
        
        //if the character doesn't exist we need to manually spawn them in.
        if (!go) {
            GameObject menuPlayer = Instantiate(PlayerPrefab, canvas.transform);
        }
        go.name = "MenuP" + currNumPlayers;
        var playerInput = go.GetComponent<PlayerInput>();
        playerInput.uiInputModule = GetComponent<InputSystemUIInputModule>();
        controlScheme.SetControlScheme(playerInput.currentControlScheme);
        
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

        mds.numPlayers = currNumPlayers;
        // Prompt the match menu to add a new player options panel
        MenuRunner.RefreshPlayerSections();
        
        x.refresh(Vector2.left);
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
