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


    // �κ��丮�� ������ �߰� (����, ����, ��� ���)
    public void AddItem(Item item)
    {
        if (!item.itemType.Equals(Item.ItemType.Equip))
        {
            // ���� �̸��� ���� �������� �ִ��� ã��
            int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

            // �������� ������ ��������
            if (itemIndex >= 0)
            {
                items[itemIndex].quantity += item.quantity;
                newItem = items[itemIndex];
                //return;
            }
            // �������� ������ �κ��丮�� �߰�
            else if (items.Count < 40)
            {
                items.Add(item);
                item.quantity = item.quantity;
                newItem = item;
            }
            else
            {
                // �κ� ����
                return;
            }
        }
        // ��� �������� �߰��� ��
        else if (items.Count < 40)
        {
            // �׳� ������ �߰�
            items.Add(item);
            item.quantity = 1;
            newItem = item;
        }
        // �κ� ����
        else
        {
            return;
        }
        OnItemAdded?.Invoke(newItem);
        newItem = null;
    }

    // ������ ���� (�Ǹ�, ����, ����, ��� ���)
    public void RemoveItem(Item item)
    {
        // ���� �̸��� ���� �������� �ִ��� ã��
        int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

        // �κ��丮�� ������ �������� �ִٸ�
        if (itemIndex >= 0)
        {
            newItem = items[itemIndex];
            // �Һ� ������
            if (item.itemType.Equals(Item.ItemType.Consume))
            {
                // ���� �ϰ� ������ 1���� �����ϰ�
                if (items[itemIndex].quantity > 0)
                {
                    items[itemIndex].quantity--;
                }
                // ������ 0�� �Ǹ� ������ ����
                if (items[itemIndex].quantity <= 0)
                {
                    items[itemIndex].quantity = 0;
                    items.RemoveAt(itemIndex);
                }
            }
            // ��� �������� �׳� ������ ����
            else
            {
                items[itemIndex].quantity = 0;
                items.RemoveAt(itemIndex);
            }
            OnItemAdded?.Invoke(newItem);
            newItem = null;
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
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = (Item)dataManager.itemDataBase.itemList[itemIndex].Clone();
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
