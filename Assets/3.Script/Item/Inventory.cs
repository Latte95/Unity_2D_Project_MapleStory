using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public List<Item> items;
    public DataManager dataManager;

    ///
    [SerializeField]
    private InventorySlot[] slot;
    [SerializeField]
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;
    private int slotCnt = 40;

    private void Awake()
    {
        slot = new InventorySlot[slotCnt];
        for (int i = 0; i < slotCnt; i++)
        {
            GameObject slotClone = Instantiate(SlotPrefabs);
            slotClone.transform.SetParent(SlotTrans, false);

            InventorySlot newSlot = slotClone.GetComponent<InventorySlot>();
            newSlot.icon.color = new Color(1, 1, 1, 0);
            newSlot.itemCount_Text.text = null;

            slot[i] = newSlot;
        }
    }

    private void UpdateSlot(Item item)
    {
        for (int i = 0; i < slotCnt; i++)
        {
            if (slot[i].icon.sprite == null)
            {
                slot[i].icon.sprite = item.itemIcon;
                slot[i].icon.color = new Color(1, 1, 1, 1);
                break;
                //    newSlot.itemCount_Text.text = items[i].quantity.ToString();
            }
            else if (slot[i].icon.sprite.Equals(item.itemIcon) && !(item is EquipableItem))
            {
                slot[i].itemCount_Text.text = "x" + item.quantity.ToString();
                break;
            }
        }
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
                //return;
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
            // ��Ÿ ������
        }
        UpdateSlot(item);
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

    public void GetItem(int itemID)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = dataManager.itemDataBase.itemList[itemIndex];
        AddItem(tmpItem);
    }
    public void GetItem(string itemName)
    {
        int itemIndex = dataManager.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = dataManager.itemDataBase.itemList[itemIndex];
        AddItem(tmpItem);
    }

}
