using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory
{
    public List<Item> items { get; private set; }

    public Inventory()
    {
        items = new List<Item>();
    }

    // �κ��丮�� ������ �߰� (����, ����, ��� ���)
    public void AddItem(Item item)
    {
        if (item is ConsumableItem)
        {
            // ���� �̸��� ���� �������� �ִ��� ã��
            int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

            // �������� ������ ��������
            if (itemIndex >= 0)
            {
                items[itemIndex].quantity++;
                return;
            }
            // �������� ������ �κ��丮�� �߰�
            else
            {
                items.Add(item);
                item.quantity = 1;
            }
        }
        // ��� �������� �߰��� ��
        else if (item is EquipableItem)
        {
            // �׳� ������ �߰�
            items.Add(item);
            item.quantity = 1;
        }
        else
        {
            // ��� �Һ� �ƴ� ������ �����ϸ� �߰�. (��Ƽ��Ʈ ��)
        }
    }

    // ������ ���� (�Ǹ�, ����, ����, ��� ���)
    public void RemoveItem(Item item)
    {
        // ���� �̸��� ���� �������� �ִ��� ã��
        int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

        // �κ��丮�� ������ �������� �ִٸ�
        if (itemIndex >= 0)
        {
            // �Һ� ������
            if (item is ConsumableItem)
            {
                // ���� �ϰ� ������ 1���� �����ϰ�
                if (items[itemIndex].quantity > 0)
                {
                    items[itemIndex].quantity--;
                }
                // ������ 0�� �Ǹ� ������ ����
                if (items[itemIndex].quantity <= 0)
                {
                    items.RemoveAt(itemIndex);
                }
            }
            // ��� �������� �׳� ������ ����
            else
            {
                items.RemoveAt(itemIndex);
            }
        }
    }
}
