using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserItem : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private ShopSlot[] slot;
    [SerializeField]
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;
    private Player player;

    private int slotCnt = 40;
    private int itemCnt = 0;

    private void Awake()
    {
        slot = new ShopSlot[slotCnt];
        for (int i = 0; i < slotCnt; i++)
        {
            GameObject slotClone = Instantiate(SlotPrefabs);
            slotClone.transform.SetParent(SlotTrans, false);

            ShopSlot newSlot = slotClone.GetComponent<ShopSlot>();
            newSlot.icon.color = new Color(1, 1, 1, 0);
            newSlot.itemName.text = null;
            newSlot.itemPrice.text = null;

            slot[i] = newSlot;
        }
    }

    private void OnEnable()
    {
        if (player != null)
        {
            InitializeSlot();
        }

        inventory.OnItemAdded += UpdateUI;
        inventory.OnItemRemoved += InitializeSlot;
        // 메소 획득 갱신
        if (GameManager.Instance != null)
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded += InitializeSlot;
        }

        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.nowPlayer;
        }
        if (player != null)
        {
            itemCnt = player.inventory.items.Count;
        }
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
        inventory.OnItemRemoved -= InitializeSlot;
        if (player != null)
        {
            player.GetComponent<PlayerControl>().OnMoneyAdded -= InitializeSlot;
        }
    }

    private void Start()
    {
        if (player != null)
        {
            inventory = player.inventory;
            InitializeSlot();
        }
        inventory.OnItemAdded += UpdateUI;
        inventory.OnItemRemoved += InitializeSlot;
    }

    private void InitializeSlot()
    {
        itemCnt = player.inventory.items.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            Item item = player.inventory.items[i];
            slot[i].icon.sprite = player.inventory.items[i].itemIcon;
            slot[i].icon.color = new Color(1, 1, 1, 1);
            slot[i].itemName.text = player.inventory.items[i]._itemName;
            slot[i].itemPrice.text = (player.inventory.items[i]._price*0.5f).ToString();
        }
        for (int i = itemCnt; i < slotCnt; i++)
        {
            slot[i].icon.sprite = null;
            slot[i].icon.color = new Color(1, 1, 1, 0);
            slot[i].itemName.text = null;
            slot[i].itemPrice.text = null;
        }
    }

    private void UpdateUI(Item newItem)
    {
        //int itemIndex = Array.FindIndex(slot, s => s.icon.sprite == newItem.itemIcon);
        int itemIndex = player.inventory.items.FindIndex(item => item._itemID == newItem._itemID);

        if (itemIndex >= 0)
        {
            //이미 있는 아이템이면 갯수만 증가
            if (GameManager.Instance.nowPlayer.inventory.items[itemIndex].quantity > 1)
            {
                slot[itemIndex].itemName.text = player.inventory.items[itemIndex]._itemName;
                slot[itemIndex].itemPrice.text = (player.inventory.items[itemIndex]._price * 0.5f).ToString();
            }
            else
            {
                slot[itemIndex].icon.sprite = newItem.itemIcon;
                slot[itemIndex].icon.color = new Color(1, 1, 1, 1);
                slot[itemIndex].itemName.text = player.inventory.items[itemIndex]._itemName;
                slot[itemIndex].itemPrice.text = (player.inventory.items[itemIndex]._price * 0.5f).ToString();
            }
        }
    }
}
