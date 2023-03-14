using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEventSysTrigger : EventTrigger
{
    public override void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("OnBeginDrag called.");
    }

    public override void OnCancel(BaseEventData data)
    {
        Debug.Log("OnCancel called.");
    }

    public override void OnDeselect(BaseEventData data)
    {
        Debug.Log("OnDeselect called.");
    }

    public override void OnMove(AxisEventData data)
    {
        Debug.Log("OnMove called.");
    }

    public override void OnScroll(PointerEventData data)
    {
        Debug.Log("OnScroll called.");
    }

    public override void OnSelect(BaseEventData data)
    {
        Debug.Log("OnSelect called.");
    }

    public override void OnSubmit(BaseEventData data)
    {
        Debug.Log("OnSubmit called.");
    }

    public override void OnUpdateSelected(BaseEventData data)
    {
        Debug.Log("OnUpdateSelected called.");
    }
}