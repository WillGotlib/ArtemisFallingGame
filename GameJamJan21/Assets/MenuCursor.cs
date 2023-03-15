using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    public int playerNumber;
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
            transform.position = new Vector3(transform.position.x - 110f, transform.position.y - 5f, transform.position.z);
            print(newTarget);    
        }
        // Action();
    }

    public void LoadCursorImage(int playerNumber) {
        GetComponent<SpriteRenderer>().sprite = cursorImages[playerNumber];
    }
}
