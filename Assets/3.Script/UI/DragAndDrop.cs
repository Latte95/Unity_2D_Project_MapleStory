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
        Debug.Log("�巡�� ����");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("���");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ����");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
