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


        inventory.OnItemAdded += UpdateUI;
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
    }

    private void Update()
    {
        UpdateSlot();
    }
    private void UpdateUI(Item newItem)
    {
        int itemIndex = Array.FindIndex(slot, s => s.icon.sprite == newItem.itemIcon);

        if (itemIndex >= 0)
        {
            // Update the existing slot
            slot[itemIndex].icon.sprite = newItem.itemIcon;
            slot[itemIndex].icon.color = new Color(1, 1, 1, 1);
            slot[itemIndex].itemCount_Text.text = newItem.quantity.ToString();
        }
        else
        {
            // Find an empty slot and update it
            for (int i = 0; i < slot.Length; i++)
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

    IEnumerator UpdateSlot_co()
    {
        yield return null;

        itemCnt = inventory.items.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            if (slot[i].icon.sprite == null)
            {
                slot[i].icon.sprite = inventory.newItem.itemIcon;
                slot[i].icon.color = new Color(1, 1, 1, 1);
                break;
            }
            else if (slot[i].icon.sprite.Equals(inventory.newItem.itemIcon) && !(inventory.newItem is EquipableItem))
            {
                slot[i].itemCount_Text.text = "x" + inventory.newItem.quantity.ToString();
                break;
            }
            //GameObject slotClone = Instantiate(SlotPrefabs);
            //slotClone.transform.SetParent(SlotTrans, false);

            //InventorySlot newSlot = slotClone.GetComponent<InventorySlot>();
            //newSlot.icon.sprite = inventory.items[i].itemIcon;
            //// 아이템 개수가 있다면
            //if (inventory.items[i].quantity > 0)
            //{
            //    newSlot.itemCount_Text.text = inventory.items[i].quantity.ToString();
            //}

            //slot[i] = newSlot;
        }
    }

    private void UpdateSlot()
    {
        itemCnt = inventory.items.Count;
        int i = 0;
        foreach(Item item in inventory.items)
        {
            if (slot[i].icon.sprite == null)
            {
                slot[i].icon.sprite = item.itemIcon;
                slot[i].icon.color = new Color(1, 1, 1, 1);
                break;
            }
            else if (slot[i].icon.sprite.Equals(item.itemIcon) && !(item is EquipableItem))
            {
                slot[i].itemCount_Text.text = "x" + item.quantity.ToString();
                break;
            }
            i++;
        }
        //for (int i = 0; i < itemCnt; i++)
        //{
        //    if (slot[i].icon.sprite == null)
        //    {
        //        slot[i].icon.sprite = item.itemIcon;
        //        slot[i].icon.color = new Color(1, 1, 1, 1);
        //        break;
        //    }
        //    else if (slot[i].icon.sprite.Equals(item.itemIcon) && !(item is EquipableItem))
        //    {
        //        slot[i].itemCount_Text.text = "x" + item.quantity.ToString();
        //        break;
        //    }
        //}
    }


    public void AddItem(int itemID)
    {
        int itemIndex = inventory.dataManager.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        Item tmpItem = inventory.dataManager.itemDataBase.itemList[itemIndex];
        inventory.AddItem(tmpItem);
    }
    public void AddItem(string itemName)
    {
        int itemIndex = inventory.dataManager.itemDataBase.itemList.FindIndex(item => item._itemName == itemName);
        Item tmpItem = inventory.dataManager.itemDataBase.itemList[itemIndex];
        inventory.AddItem(tmpItem);
    }
}
