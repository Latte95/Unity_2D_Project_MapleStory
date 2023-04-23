using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory
{
    public List<Item> items;
    //public DataManager dataManager;
    public DataManager dataManager
    {
        get
        {
            if (DataManager.instance != null)
            {
                return DataManager.instance;
            }
            else
            {
                Debug.LogError("DataManager instance not found.");
                return null;
            }
        }
    }

    public Item newItem;
    public event Action<Item> OnItemAdded;
    public event Action OnItemRemoved;


    // 인벤토리에 아이템 추가 (구매, 착용, 드랍 등등)
    public void AddItem(Item item)
    {
        if (!item.itemType.Equals(Item.ItemType.Equip))
        {
            // 같은 이름을 가진 아이템이 있는지 찾기
            int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

            // 아이템이 있으면 수량증가
            if (itemIndex >= 0)
            {
                items[itemIndex].quantity += item.quantity;
                newItem = items[itemIndex];
                //return;
            }
            // 아이템이 없으면 인벤토리에 추가
            else if (items.Count < 40)
            {
                items.Add(item);
                item.quantity = item.quantity;
                newItem = item;
            }
            else
            {
                // 인벤 꽉참
                return;
            }
        }
        // 장비 아이템을 추가할 땐
        else if (items.Count < 40)
        {
            // 그냥 아이템 추가
            items.Add((EquipableItem)item);
            item.quantity = 1;
            newItem = (EquipableItem)item;
        }
        // 인벤 꽉참
        else
        {
            return;
        }
        OnItemAdded?.Invoke(newItem);
        newItem = null;
    }

    // 아이템 삭제 (판매, 해제, 버림, 사용 등등)
    public void RemoveItem(Item item)
    {
        // 같은 이름을 가진 아이템이 있는지 찾기
        int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

        // 인벤토리에 삭제할 아이템이 있다면
        if (itemIndex >= 0)
        {
            newItem = items[itemIndex];
            // 소비 아이템
            if (item.itemType.Equals(Item.ItemType.Consume))
            {
                // 보유 하고 있으면 1개씩 제거하고
                if (items[itemIndex].quantity > 0)
                {
                    items[itemIndex].quantity--;
                    OnItemAdded?.Invoke(newItem);
                }
                // 수량이 0이 되면 완전히 제거
                if (items[itemIndex].quantity <= 0)
                {
                    items[itemIndex].quantity = 0;
                    items.RemoveAt(itemIndex);
                }
            }
            // 장비 아이템은 그냥 완전히 제거
            else
            {
                items[itemIndex].quantity = 0;
                items.RemoveAt(itemIndex);
            }
            newItem = null;
            OnItemRemoved?.Invoke();
        }
    }

    public void DropItem(int itemId, int cnt)
    {
        // 같은 이름을 가진 아이템이 있는지 찾기
        int itemIndex = items.FindIndex(invenItem => invenItem._itemID == itemId);
        if(items[itemIndex].quantity < cnt)
        {
            return;
        }

        // 인벤토리에 삭제할 아이템이 있다면
        if (itemIndex >= 0)
        {
            newItem = items[itemIndex];
            // 버릴 개수만큼 제거
            if (items[itemIndex].quantity > 0)
            {
                items[itemIndex].quantity -= cnt;
                OnItemAdded?.Invoke(newItem);
            }
            // 수량이 0이 되면 완전히 제거
            if (items[itemIndex].quantity <= 0)
            {
                items[itemIndex].quantity = 0;
                items.RemoveAt(itemIndex);
            }
            newItem = null;
        }
    }

    public void GetItem(int itemID)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }
    public void GetItem(string itemName)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }

    public void GetItem(int itemID, int quantity)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        if (tmpItem.itemType.Equals(Item.ItemType.Consume))
        {
            tmpItem.quantity = quantity;
            AddItem(tmpItem);
        }
        else if (tmpItem.itemType.Equals(Item.ItemType.Equip))
        {
            for (int i = 0; i < quantity; i++)
            {
                AddItem(tmpItem);
            }
        }
    }
    public void GetItem(string itemName, int quantity)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        if (tmpItem.itemType.Equals(Item.ItemType.Consume))
        {
            tmpItem.quantity = quantity;
            AddItem(tmpItem);
        }
        else if (tmpItem.itemType.Equals(Item.ItemType.Equip))
        {
            for (int i = 0; i < quantity; i++)
            {
                AddItem(tmpItem);
            }
        }
    }

}
