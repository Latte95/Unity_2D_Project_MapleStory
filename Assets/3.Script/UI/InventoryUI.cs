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

        // Initialize the slots based on the current inventory items
        for (int i = 0; i < itemCnt; i++)
        {
            UpdateUI(inventory.items[i]);
        }

        inventory.OnItemAdded += UpdateUI;
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
    }

    private void UpdateUI(Item newItem)
    {
        int itemIndex = Array.FindIndex(slot, s => s.icon.sprite == newItem.itemIcon);
        if(newItem is EquipableItem)
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
