using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text itemCount_Text;

    public void AddItem(Item _item)
    {
        icon.sprite = _item.itemIcon;
        if(!Item.ItemType.Equip.Equals(_item.itemType))
        {
            if(_item.quantity > 0)
            {
                itemCount_Text.text = "x " + itemCount_Text.ToString();
            }
            else
            {
                itemCount_Text.text = "";
            }
        }
    }

    public void RemoveItem()
    {
        icon.sprite = null;
        itemCount_Text.text = "";
    }
}