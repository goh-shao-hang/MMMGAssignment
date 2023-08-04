using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 TouchDelta { get; private set; }

    private bool _pressed;

    void Update()
    {
        if (_pressed)
        {
            if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
            {
                TouchDelta = Touchscreen.current.touches[0].delta.ReadValue();
            }
            else
            {
                TouchDelta = Vector2.zero;
            }
        }
        else
        {
            TouchDelta = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

}