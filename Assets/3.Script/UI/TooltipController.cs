using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 툴팁 띄울 슬롯들이 가지고 있는 스크립트
public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemTooltip tooltip;
    public Image icon;

    private void Start()
    {
        tooltip = GameObject.FindGameObjectWithTag("UI").transform.Find("Tooltip").GetComponent<ItemTooltip>();
        TryGetComponent(out icon);
    }
    // 창 사라지면 툴팁도 끔
    private void OnDisable()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이콘 있을 경우만 툴팁 표시
        if(icon.sprite == null)
        {
            return;
        }
        string itemID = icon.sprite.name;

        if(itemID != null)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.SetUpTootip(int.Parse(itemID));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}
