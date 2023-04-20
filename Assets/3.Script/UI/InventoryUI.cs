using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private InventorySlot[] slot;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;

    private int slotCnt = 40;
    private int itemCnt;

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

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        // �� ��ȯ�� ������ ����. �߻� ������ �𸣰���...
        if (player == null)
        {
            return;
        }
        inventory = player.inventory;
        itemCnt = player.inventory.items.Count;

        // �κ��丮 ����, �κ��丮 ���� ���·� �߰��� �������� �����ϱ� ����
        InitializeSlot();
        // �κ��丮 ���� ���·� ������ �߰��� ���Կ� �ٷιٷ� ä����
        inventory.OnItemAdded += UpdateUI;
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
    }

    private void InitializeSlot()
    {
        itemCnt = player.inventory.items.Count;
        // ���� ������ ���� �Ҵ�
        for (int i = 0; i < itemCnt; i++)
        {
            slot[i].icon.sprite = player.inventory.items[i].itemIcon;
            slot[i].icon.color = new Color(1, 1, 1, 1);
            if (player.inventory.items[i].quantity > 1)
            {
                slot[i].itemCount_Text.text = "x" + player.inventory.items[i].quantity.ToString();
            }
            else
            {
                slot[i].itemCount_Text.text = null;
            }
        }
        // �̺��� ������ �ʱ�ȭ
        // 10��° ���Կ� �������� �ִµ�, ������ ����� 9�������� ��� 10��° ���Կ� �������� �����ִ� �� ����
        for(int i = itemCnt; i < slotCnt; i++)
        {
            slot[i].icon.sprite = null;
            slot[i].icon.color = new Color(1, 1, 1, 0);
            slot[i].itemCount_Text.text = null;
        }
    }

    private void UpdateUI(Item newItem)
    {
        if(newItem == null)
        {
            return;
        }
        int itemIndex = Array.FindIndex(slot, s => s.icon.sprite == newItem.itemIcon);
        if (newItem is EquipableItem)
        {
            itemIndex = -1;
        }


        if (itemIndex >= 0)
        {
            if (slot[itemIndex].icon.sprite.name.Equals(newItem.itemIcon.name))
            {
                InitializeSlot();
                return;
            }
            // Update the existing slot
            slot[itemIndex].icon.sprite = newItem.itemIcon;
            slot[itemIndex].icon.color = new Color(1, 1, 1, 1);
            if (player.inventory.items[itemIndex].quantity > 1)
            {
                slot[itemIndex].itemCount_Text.text = "x" + player.inventory.items[itemIndex].quantity.ToString();
            }
            else if (player.inventory.items[itemIndex].quantity.Equals(1))
            {
                slot[itemIndex].itemCount_Text.text = null;
            }
            else
            {
                slot[itemIndex].icon.sprite = null;
                slot[itemIndex].icon.color = new Color(1, 1, 1, 0);
                slot[itemIndex].itemCount_Text.text = null;
            }
        }
        else
        {
            int length = slot.Length;
            for (int i = 0; i < length; i++)
            {
                if (slot[i].icon.sprite == null)
                {
                    slot[i].icon.sprite = newItem.itemIcon;
                    slot[i].icon.color = new Color(1, 1, 1, 1);
                    slot[i].itemCount_Text.text = null;
                    if (player.inventory.items[i].quantity > 1)
                    {
                        slot[i].itemCount_Text.text = "x" + player.inventory.items[i].quantity.ToString();
                    }
                    break;
                }
            }
        }
    }

    public void InitializePlayer()
    {
        player = FindObjectOfType<Player>();

        if (player == null)
        {
            Debug.LogWarning("Player object not found in the scene.");
        }
    }
}
