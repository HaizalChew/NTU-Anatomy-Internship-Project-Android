using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ControllableSlider : Slider , IBeginDragHandler, IEndDragHandler
{
    public bool isDragged;
    public UnityEvent OnEndValueChanged = new UnityEvent();

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragged = false;
        OnEndValueChanged.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isDragged = true;
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        OnEndValueChanged.Invoke();
    }
}
