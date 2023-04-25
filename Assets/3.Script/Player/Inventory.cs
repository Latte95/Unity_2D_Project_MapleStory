using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory
{
    public List<Item> items;

    public Item newItem;
    public event Action OnItemAdded;
    public event Action OnItemRemoved;


    // �κ��丮�� ������ �߰� (����, ����, ��� ���)
    public void AddItem(Item item)
    {
        if (!item.itemType.Equals(Item.ItemType.Equip))
        {
            // ���� �̸��� ���� �������� �ִ��� ã��
            int itemIndex = items.FindIndex(invenItem => invenItem._itemName.Equals(item._itemName));

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
            items.Add((EquipableItem)item);
            item.quantity = 1;
            newItem = (EquipableItem)item;
        }
        // �κ� ����
        else
        {
            return;
        }
        OnItemAdded?.Invoke();
        newItem = null;
    }

    // ������ ���� (�Ǹ�, ����, ����, ��� ���)
    public void RemoveItem(Item item)
    {
        // ���� �̸��� ���� �������� �ִ��� ã��
        int itemIndex = items.FindIndex(invenItem => invenItem._itemName.Equals(item._itemName));

        // �κ��丮�� ������ �������� �ִٸ�
        if (itemIndex >= 0)
        {
            newItem = items[itemIndex];
            // �Һ� ������
            if (!item.itemType.Equals(Item.ItemType.Equip))
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
            newItem = null;
            OnItemRemoved?.Invoke();
        }
    }

    public void DropItem(int itemId, int cnt)
    {
        // ���� �̸��� ���� �������� �ִ��� ã��
        int itemIndex = items.FindIndex(invenItem => invenItem._itemID.Equals(itemId));
        if(items[itemIndex].quantity < cnt)
        {
            return;
        }

        // �κ��丮�� ������ �������� �ִٸ�
        if (itemIndex >= 0)
        {
            newItem = items[itemIndex];
            // ���� ������ŭ ����
            if (items[itemIndex].quantity > 0)
            {
                items[itemIndex].quantity -= cnt;
                OnItemAdded?.Invoke();
            }
            // ������ 0�� �Ǹ� ������ ����
            else
            {
                items[itemIndex].quantity = 0;
                items.RemoveAt(itemIndex);
            }
            newItem = null;
        }
    }

    public void GetItem(int itemID)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID.Equals(itemID));
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }
    public void GetItem(string itemName)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemName.Equals(itemName));
        Item tmpItem = (Item)DataManager.instance.itemDataBase.itemList[itemIndex].Clone();
        AddItem(tmpItem);
    }

    public void GetItem(int itemID, int quantity)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID.Equals(itemID));
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
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemName.Equals(itemName));
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