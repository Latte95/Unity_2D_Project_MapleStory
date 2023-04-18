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
    //    Debug.Log("드래그 시작");
    //    previousParent = transform.parent;

    //    transform.SetParent(canvas);
    //    // 안 가려지도록 마지막 자식으로 설정
    //    transform.SetAsLastSibling();

    //    canvasGroup.alpha = 0.6f;
    //    // 레이캐스트에 감지 안 되도록
    //    canvasGroup.blocksRaycasts = false;
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("드래그 중");
    //    rect.position = eventData.position;
    //}

    //public void OnDrop(PointerEventData eventData)
    //{
    //    Debug.Log("드랍");
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("드래그 종료");
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
