using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    private GameObject currentlySelected;

    [SerializeField] private Sprite[] cursorImages;

    public EventSystem _eventSys;

    
    public void Update() {
        
    }

    public void OnNavigate() {
        // print("Navigated");
        GameObject newTarget = _eventSys.currentSelectedGameObject;
        // TODO: Doing this every frame is wasteful. it would be good to go through proper channels (cued by player NAVIGATION)
        if (newTarget != currentlySelected) {
            currentlySelected = newTarget;
            transform.position = newTarget.transform.position;
            RectTransform x = newTarget.GetComponent<RectTransform>();
            if (x) {
                print(transform.position.x + 2);
            }
            print(newTarget);    
        }
        // Action();
    }

    public void LoadCursorImage(int playerNumber) {
        GetComponent<SpriteRenderer>().sprite = cursorImages[playerNumber];
    }
}
