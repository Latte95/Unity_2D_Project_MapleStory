using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Vector3 cameraPosition;

    public Image icon;

    private void Awake()
    {
        cameraPosition = Camera.main.transform.position;
        TryGetComponent(out icon);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("드랍");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 종료");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
