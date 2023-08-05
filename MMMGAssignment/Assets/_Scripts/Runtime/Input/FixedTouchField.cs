using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 TouchDelta { get; private set; }

    private bool _pressed;
    private int _touchID;
    private Vector2 _previousTouchPosition;

    void Update()
    {
        if (_pressed)
        {
            if (_touchID >= 0 && _touchID < Input.touches.Length)
            {
                TouchDelta = Input.touches[_touchID].position - _previousTouchPosition;
                _previousTouchPosition = Input.touches[_touchID].position;
            }
            else
            {
                TouchDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _previousTouchPosition;
            }

            //New input system approach, but with a flaw that only the first touch works
            /*if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
            {
                Touchscreen.current.touches[0].touchId
                TouchDelta = Touchscreen.current.touches[0].delta.ReadValue();
            }
            else
            {
                TouchDelta = Vector2.zero;
            }*/
        }
        else
        {
            TouchDelta = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _touchID = eventData.pointerId;
        _previousTouchPosition = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

}