using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Vector2 _initialPosition;
    [SerializeField]
    private Transform parentUI;

    private void Awake()
    {
        parentUI = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = parentUI.position - new Vector3(eventData.position.x, eventData.position.y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentUI.position = new Vector3(eventData.position.x, eventData.position.y, 0) + new Vector3(_initialPosition.x, _initialPosition.y, 0);
    }
}
