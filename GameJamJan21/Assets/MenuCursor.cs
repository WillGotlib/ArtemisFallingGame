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
        if (!currentlyMoving && vec.magnitude > 0.6) {
            currentlyMoving = true;
            print("Navigated" + value.Get<Vector2>());
            refresh(vec);
        } else if (vec.magnitude < 0.1) {
            currentlyMoving = false;
        }
        // Action();
    }

    public void OnEnter() {
        print("We pressed enter");
         _eventSys.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void refresh(Vector2 dir) {
        GameObject newTarget = _eventSys.currentSelectedGameObject;
        if (newTarget != currentlySelected) {
            currentlySelected = newTarget;
            MoveToTarget(newTarget, new Vector3(-110f, -5f, 0));
        } else if (dir.magnitude != 0) {
            Vector3 dir3 = new Vector3(dir.x , dir.y, 0);
            Selectable currSelectable = currentlySelected.GetComponent<Selectable>().FindSelectable(dir3);
            print(currSelectable);
            if (currSelectable) {
                _eventSys.SetSelectedGameObject(currSelectable.gameObject);
                currentlySelected = currSelectable.gameObject;
                MoveToTarget(currentlySelected, new Vector3(-110f, -5f, 0));
                print("Had to manually make the move. Now on " + currSelectable);
            }
        }
        // Do a check for if there are any hovering cursors with a non-selected button
        // manager.SelectionCheck();
    }

    public void MoveToTarget(GameObject newTarget, Vector3 offset) {
        transform.position = newTarget.transform.position + offset;;
        // RectTransform x = newTarget.GetComponent<RectTransform>();
        // transform.position = transform.position 
        print(newTarget);    
    }

    public void LoadCursorImage(int playerNumber) {
        GetComponent<Image>().sprite = cursorImages[playerNumber];
    }

    public void setManager(PlayerManagerUI man) {
        manager = man;
    }
}
