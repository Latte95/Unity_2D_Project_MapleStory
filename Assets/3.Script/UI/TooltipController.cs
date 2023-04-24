using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemTooltip tooltip;
    public Image icon;

    private void Start()
    {
        tooltip = GameObject.Find("Canvas").transform.Find("Tooltip").GetComponent<ItemTooltip>();
        TryGetComponent(out icon);
    }
    private void OnDisable()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
