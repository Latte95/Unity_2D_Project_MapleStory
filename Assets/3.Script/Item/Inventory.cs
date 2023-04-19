using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory
{
    [SerializeField]
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


    // 인벤토리에 아이템 추가 (구매, 착용, 드랍 등등)
    public void AddItem(Item item)
    {
        if (item is ConsumableItem)
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
            else
            {
                items.Add(item);
                item.quantity = item.quantity;
                newItem = item;
            }
        }
        // 장비 아이템을 추가할 땐
        else if (item is EquipableItem)
        {
            // 그냥 아이템 추가
            items.Add(item);
            item.quantity = 1;
            newItem = item;
        }
        else
        {
            // 기타 아이템
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
            // 소비 아이템
            if (item is ConsumableItem)
            {
                // 보유 하고 있으면 1개씩 제거하고
                if (items[itemIndex].quantity > 0)
                {
                    items[itemIndex].quantity--;
                }
                // 수량이 0이 되면 완전히 제거
                if (items[itemIndex].quantity <= 0)
                {
                    items.RemoveAt(itemIndex);
                }
            }
            // 장비 아이템은 그냥 완전히 제거
            else
            {
                items.RemoveAt(itemIndex);
            }
        }
    }

    public void GetItem(int itemID)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = (Item)dataManager.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }
    public void GetItem(string itemName)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = (Item)dataManager.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }

    public void GetItem(int itemID, int quantity)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = (Item)dataManager.itemDataBase.itemList[itemIndex].Clone();
        if (tmpItem is ConsumableItem)
        {
            tmpItem.quantity = quantity;
            AddItem(tmpItem);
        }
        else if (tmpItem is EquipableItem)
        {
            for (int i = 0; i < quantity; i++)
            {
                AddItem(tmpItem);
            }
        }
    }
    public void GetItem(string itemName, int quantity)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = (Item)dataManager.itemDataBase.itemList[itemIndex].Clone();
        if (tmpItem is ConsumableItem)
        {
            tmpItem.quantity = quantity;
            AddItem(tmpItem);
        }
        else if (tmpItem is EquipableItem)
        {
            for (int i = 0; i < quantity; i++)
            {
                AddItem(tmpItem);
            }
        }
    }

}
