using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer icon;

    public void SetItem(Item _item)
    {
        item._itemName = _item._itemName;
        item.itemIcon = _item.itemIcon;
        item.itemType = _item.itemType;

        icon.sprite = item.itemIcon;
    }

    public Item GetItem()
    {
        return item;
    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
