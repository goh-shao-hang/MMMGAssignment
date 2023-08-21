using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsShootButtonHeld { get; private set; } = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Held");
        IsShootButtonHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Relesaed");
        IsShootButtonHeld = false;
    }

}
