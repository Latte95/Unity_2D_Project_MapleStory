using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ���� ��� ���Ե��� ������ �ִ� ��ũ��Ʈ
public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemTooltip tooltip;
    public Image icon;

    private void Start()
    {
        tooltip = GameObject.FindGameObjectWithTag("UI").transform.Find("Tooltip").GetComponent<ItemTooltip>();
        TryGetComponent(out icon);
    }
    // â ������� ������ ��
    private void OnDisable()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������ ���� ��츸 ���� ǥ��
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
