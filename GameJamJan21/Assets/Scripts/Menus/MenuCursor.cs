using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuCursor : MonoBehaviour
{
    public int playerNumber;
    public GameObject currentlySelected;

    [SerializeField] private Sprite[] cursorImages;

    public EventSystem _eventSys;

    private bool currentlyMoving;

    private PlayerManagerUI manager;
    
    public void Update() {
        
    }

    public void OnNavigate(InputValue value) {
        Vector2 vec = value.Get<Vector2>();
        if (!currentlyMoving && vec.magnitude > 0.5) {
            currentlyMoving = true;
            print("Navigated" + value.Get<Vector2>() + " to " + _eventSys.currentSelectedGameObject);
            // print(GetComponent<PlayerInput>().currentControlScheme);
            StartCoroutine(waitTest(vec));
            // manager.SelectionCheck();
        } else if (vec.magnitude < 0.05) {
            currentlyMoving = false;
        }
        
    }

    public void OnEnter() {
        print("We pressed enter");
        if (playerNumber > 0) 
            _eventSys.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void refresh(Vector2 dir) {
        // Vector3 offsetVector = new Vector3(-110f, -5f, 0);
        Vector3 offsetVector = Vector3.zero;
        // TODO: MAKE THIS BETTER. RIGHT NOW IT OBSTRUCTS.
        
        GameObject newTarget = _eventSys.currentSelectedGameObject;
        if (!currentlySelected) {
            newTarget = manager.GetCurrentMenuDefault().gameObject;
        }
        if (newTarget != currentlySelected) {
            print("Cursor Target didn't match: cursor had " + currentlySelected + ", ES had " + newTarget);
            currentlySelected = newTarget;
            MoveToTarget(newTarget, offsetVector);
        } else if (dir.magnitude != 0) {
            Vector3 dir3 = new Vector3(dir.x , dir.y, 0);
            Selectable currSelectable = currentlySelected.GetComponent<Selectable>().FindSelectable(dir3);
            print(currSelectable);
            if (currSelectable) {
                _eventSys.SetSelectedGameObject(currSelectable.gameObject);
                currentlySelected = currSelectable.gameObject;
                print("Had to manually make the move. Now on " + currSelectable);
            } else {
                currSelectable = manager.GetCurrentMenuDefault();
            }
            MoveToTarget(currentlySelected, offsetVector);
        }
        // Do a check for if there are any hovering cursors with a non-selected button
        // manager.SelectionCheck();
    }

    IEnumerator waitTest(Vector2 vec)
    {
        // Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(0.05f);
        manager.SelectionCheck(playerNumber);
        refresh(vec);
        // print("\tTerminating coroutine");
    }

    public void MoveToTarget(GameObject newTarget, Vector3 offset) {
        transform.position = newTarget.transform.position + offset;;
        // RectTransform x = newTarget.GetComponent<RectTransform>();
        // transform.position = transform.position 
        print("New target: " + newTarget);
        // StartCoroutine(waitTest());
        // manager.SelectionCheck();
    }

    public void LoadCursorImage(int playerNumber) {
        GetComponent<Image>().sprite = cursorImages[playerNumber];
    }

    public void setManager(PlayerManagerUI man) {
        manager = man;
    }
}
