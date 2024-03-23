using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action onClickHandler = null;
    public Action onPressedHandler = null;
    public Action onPointerDownHandler = null;
    public Action onPointerUpHandler = null;

    bool _pressed = false;

    private void Update()
    {
        if (_pressed)
            onPressedHandler?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        onPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        onPointerUpHandler?.Invoke();
    }
}