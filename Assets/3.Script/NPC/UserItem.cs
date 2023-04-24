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
            newSlot.itemQuantity.text = null;

            slot[i] = newSlot;
        }
    }

    private void OnEnable()
    {
        if (player != null)
        {
            InitializeSlot();
        }

        inventory.OnItemAdded += InitializeSlot;
        inventory.OnItemRemoved += InitializeSlot;
        // ¸Þ¼Ò È¹µæ °»½Å
        if (GameManager.Instance != null && GameManager.Instance.nowPlayer != null)
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
        inventory.OnItemAdded -= InitializeSlot;
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
        inventory.OnItemAdded += InitializeSlot;
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
            slot[i].itemQuantity.text = "x" + player.inventory.items[i].quantity.ToString();
        }
        for (int i = itemCnt; i < slotCnt; i++)
        {
            slot[i].icon.sprite = null;
            slot[i].icon.color = new Color(1, 1, 1, 0);
            slot[i].itemName.text = null;
            slot[i].itemPrice.text = null;
            slot[i].itemQuantity.text = null;
        }
    }
}
