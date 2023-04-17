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
        mds.tutorial = false;
        //print("Starting");
        // Instantiate(PlayerPrefab);
        // OnPlayerJoined(); // Call this once
    }

    private int frameCount;
    void Update() {
        frameCount++;
        if (frameCount % 50 == 0) {
            frameCount = 0;
            SelectionCheck();
        }
    }

    public void RefreshCursors(GameObject next) {
        for (int i = 0; i < playerCursors.Count; i++) {
            print("player " + playerCursors[i] + " selected " + next);
            UpdateEventSystemTarget(i, next);
            // playerCursors[i].refresh(Vector2.zero);
            playerCursors[i].MoveToTarget(next, playerCursors[i].offsetVector);
        }
    }

    public void UpdateEventSystemTarget(int playerNumber, GameObject next) {
        playerES[playerNumber].SetSelectedGameObject(next);
    }

    public void SelectionCheck() {
        for (int i = 0; i < playerCursors.Count && i < mds.numPlayers; i++) {
            UpdateEventSystemTarget(i, previousTargets[i]);
            SelectionCheck(i);
        }
    }

    public void SelectionCheck(int playerNumber) {
        // Make sure that if there's a cursor beside a Selectable, it's selected.

        if (playerES[playerNumber].currentSelectedGameObject != playerCursors[playerNumber].currentlySelected) {
            // If the cursor and the eventsystem disagree.

            print($"[P {playerNumber}] Disagreement. ES says: {playerES[playerNumber].currentSelectedGameObject}" + 
                    $", Cursor says: {playerCursors[playerNumber].currentlySelected}. Accepting cursor.");

            previousTargets[playerNumber] = playerCursors[playerNumber].currentlySelected;
            UpdateEventSystemTarget(playerNumber, playerCursors[playerNumber].currentlySelected);
            
            playerCursors[playerNumber].MoveToTarget(playerCursors[playerNumber].currentlySelected, playerCursors[playerNumber].offsetVector);
        }
    }

    public Button GetCurrentMenuDefault() {
        return MenuRunner.GetCurrentMenuDefault();
    }

    void OnPlayerJoined() {
        print("Player Joined");
        if (currNumPlayers >= mds.maxPlayers) {
            print("Too many players! WE'VE GOTTA SHUT DOWN!!!");
            return;
        }
        GameObject newPlayer = Instantiate(MPEventSystem, transform);
        MultiplayerEventSystem playerEventSys = newPlayer.GetComponent<MultiplayerEventSystem>();
        playerEventSys.firstSelectedGameObject = GetCurrentMenuDefault().gameObject;
        playerEventSys.playerRoot = canvas;
        playerEventSys.SetSelectedGameObject(playerEventSys.firstSelectedGameObject);
        playerES.Add(playerEventSys);

        previousTargets[currNumPlayers] = playerEventSys.firstSelectedGameObject;

        currNumPlayers++;
        newPlayer.name = "MenuEventSystem P" + currNumPlayers;

        GameObject cursor = GameObject.Find ("MenuPlayer(Clone)"); // Temp
        
        //if the character doesn't exist we need to manually spawn them in.
        if (!cursor) {
            cursor = Instantiate(PlayerPrefab, canvas.transform);
        }
        cursor.name = "MenuP" + currNumPlayers;
        var playerInput = cursor.GetComponent<PlayerInput>();
        playerInput.uiInputModule = GetComponent<InputSystemUIInputModule>();

        print("Control Scheme" + controlScheme);
        var controls = controlScheme;
        var input = playerInput.currentControlScheme;
        print("Input" + input);
        controls.SetControlScheme(input);
        MenuCursor x = cursor.GetComponent<MenuCursor>();
        x.playerNumber = currNumPlayers - 1;
        x.LoadCursorImage(currNumPlayers - 1);
        print("GETTING EVENT SYSTEM: " + newPlayer.GetComponent<EventSystem>());
        x._eventSys = playerEventSys;
        print("NEW ROOT: " + playerEventSys);
        cursor.transform.SetParent(canvas.transform);
        cursor.transform.localScale = new Vector3(100, 100, 100);
        x.setManager(this);
        playerCursors.Add(x);

        InputSystemUIInputModule inputModule = newPlayer.GetComponent<InputSystemUIInputModule>();
        inputModule.actionsAsset = playerInput.actions;

        Debug.Log(cursor.name + " added.");

        mds.numPlayers = currNumPlayers;
        // Prompt the match menu to add a new player options panel
        MenuRunner.RefreshPlayerSections();
        
        x.refresh(Vector2.left);
        x.MoveToTarget(playerEventSys.firstSelectedGameObject, x.offsetVector);
    }

    void Awake() {
        reset();
        // for (int i = 0; i < currNumPlayers; i++) {
        //     OnPlayerJoined();
        // }
        // GameObject cursor = GameObject.Find("MenuP1");
        // print("CURSOR:" + cursor);
        // RefreshCursors(GetCurrentMenuDefault().gameObject);
    }

    void reset() {
        currNumPlayers = 0;
    }
}
