using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject clickUp, clickDown;

    public void OnPointerDown (PointerEventData eventData)
    {
        GameManager.Instance.CollectByTap (eventData.position, transform);
        clickUp.SetActive(true);
        clickDown.SetActive(false);
    }
    public void OnPointerUp (PointerEventData eventData)
    {
        clickUp.SetActive(false);
        clickDown.SetActive(true);
    }
}
