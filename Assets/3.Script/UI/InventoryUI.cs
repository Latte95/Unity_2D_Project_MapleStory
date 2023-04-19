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
        inventory = player.inventory;
        itemCnt = player.inventory.items.Count;

        // 인벤토리 정렬, 인벤토리 꺼진 상태로 추가된 아이템을 갱신하기 위함
        InitializeSlot();
        // 인벤토리 켜진 상태로 아이템 추가시 슬롯에 바로바로 채워짐
        inventory.OnItemAdded += UpdateUI;
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
    }

    private void InitializeSlot()
    {
        itemCnt = player.inventory.items.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            slot[i].icon.sprite = player.inventory.items[i].itemIcon;
            slot[i].icon.color = new Color(1, 1, 1, 1);
            slot[i].itemCount_Text.text = player.inventory.items[i].quantity.ToString();
            
        }
    }

    private void UpdateUI(Item newItem)
    {
        int itemIndex = Array.FindIndex(slot, s => s.icon.sprite == newItem.itemIcon);
        if (newItem is EquipableItem)
        {
            itemIndex = -1;
        }

        if (itemIndex >= 0)
        {
            // Update the existing slot
            slot[itemIndex].icon.sprite = newItem.itemIcon;
            slot[itemIndex].icon.color = new Color(1, 1, 1, 1);
            slot[itemIndex].itemCount_Text.text = newItem.quantity.ToString();
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
                    slot[i].itemCount_Text.text = newItem.quantity.ToString();
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
