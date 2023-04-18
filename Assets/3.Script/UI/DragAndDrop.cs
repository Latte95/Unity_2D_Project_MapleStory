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

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Debug.Log("�巡�� ����");
    //    previousParent = transform.parent;

    //    transform.SetParent(canvas);
    //    // �� ���������� ������ �ڽ����� ����
    //    transform.SetAsLastSibling();

    //    canvasGroup.alpha = 0.6f;
    //    // ����ĳ��Ʈ�� ���� �� �ǵ���
    //    canvasGroup.blocksRaycasts = false;
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("�巡�� ��");
    //    rect.position = eventData.position;
    //}

    //public void OnDrop(PointerEventData eventData)
    //{
    //    Debug.Log("���");
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("�巡�� ����");
    //    if(transform.Equals(canvas))
    //    {
    //        transform.SetParent(previousParent);
    //        rect.position = previousParent.GetComponent<RectTransform>().position;
    //    }
    //    canvasGroup.alpha = 1.0f;
    //    canvasGroup.blocksRaycasts = true;
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
