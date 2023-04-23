using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : MonoBehaviour
{
    [SerializeField]
    private ShopSlot[] slot;
    [SerializeField]
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;
    private Player player;

    private int slotCnt = 10;
    private int itemCnt = 0;

    public ShopUI shopUi;
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
        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.nowPlayer;
        }

        InitializeSlot();

        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.nowPlayer;
            GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded += InitializeSlot;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.GetComponent<PlayerControl>().OnMoneyAdded -= InitializeSlot;
        }
    }

    public void InitializeSlot()
    {
        StartCoroutine(nameof(InitializeSlot_co));
    }

    private IEnumerator InitializeSlot_co()
    {
        yield return new WaitForSeconds(0.01f);
        itemCnt = shopUi.items.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID == shopUi.items[i]);
            Item item = DataManager.instance.itemDataBase.itemList[itemIndex];
            slot[i].icon.sprite = item.itemIcon;
            slot[i].icon.color = new Color(1, 1, 1, 1);
            slot[i].itemName.text = item._itemName;
            slot[i].itemPrice.text = item._price.ToString();
        }
        for (int i = itemCnt; i < slotCnt; i++)
        {
            slot[i].icon.sprite = null;
            slot[i].icon.color = new Color(1, 1, 1, 0);
            slot[i].itemName.text = null;
            slot[i].itemPrice.text = null;
        }
    }
}
