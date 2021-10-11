using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickController : MonoBehaviour
{
    public UnityEvent onClickEvent;
    public UnityEvent onClickReleaseEvent;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            onClickEvent?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            onClickReleaseEvent?.Invoke();
        }
    }
}