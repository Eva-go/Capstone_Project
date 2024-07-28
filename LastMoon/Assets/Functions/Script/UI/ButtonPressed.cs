using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isButtonPressed, isButtonActive;
    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isButtonActive = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isButtonActive = false;
    }
}
