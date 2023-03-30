using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerManagerUI : MonoBehaviour
{
    public MatchDataScriptable mds;
    
    [SerializeField] GameObject MPEventSystem;
    [SerializeField] MatchMenu PreMatchMenu;
    [SerializeField] GameObject[] DefaultFirstSelected;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] CurrentControlScheme controlScheme;

    public static int currNumPlayers = 0;

    private List<MenuCursor> playerCursors = new List<MenuCursor>();
    private List<MultiplayerEventSystem> playerES = new List<MultiplayerEventSystem>();
    
    // Update is called once per frame
    void Start()
    {
        mds.numPlayers = currNumPlayers;
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
        // Make sure that if there's a cursor beside a Selectable, it's selected
        for (int i = 0; i < playerCursors.Count; i++) {
            print("CURRENTLY SELECTED: " + playerES[i].currentSelectedGameObject);
            // playerCursors[i].MoveToTarget(playerES[i].currentSelectedGameObject, new Vector3(0, 0, 0));
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
        x.refresh(Vector2.zero);
        mds.numPlayers = currNumPlayers;

        // Prompt the match menu to add a new player options panel
        PreMatchMenu.RefreshPlayerSections(currNumPlayers);
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
