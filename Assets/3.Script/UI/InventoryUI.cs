using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public enum slotTab
    {
        Equip, Consume, Etc, Cnt,
    }
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private InventorySlot[] slot;
    [SerializeField]
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;
    public int tab;

    private int slotCnt = 40;
    private int itemCnt;

    private Text moneyText;

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
        tab = 0;
    }

    private void OnEnable()
    {
        InitializeSlot();
        // 인벤토리 켜진 상태로 아이템 추가시 슬롯에 바로바로 채워짐
        inventory.OnItemAdded += InitializeSlot;
        inventory.OnItemRemoved += InitializeSlot;
        // 메소 획득 갱신
        if (GameManager.Instance.player != null)
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded += InitializeSlot;
        }
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= InitializeSlot;
        inventory.OnItemRemoved -= InitializeSlot;
        if (GameManager.Instance != null && GameManager.Instance.nowPlayer != null)
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded -= InitializeSlot;
        }
    }

    private void Start()
    {
        inventory = GameManager.Instance.nowPlayer.inventory;
        itemCnt = GameManager.Instance.nowPlayer.inventory.items.Count;
        // 인벤토리 정렬, 인벤토리 꺼진 상태로 추가된 아이템을 갱신하기 위함
        InitializeSlot();
        inventory.OnItemAdded += InitializeSlot;
        inventory.OnItemRemoved += InitializeSlot;
    }

    private void InitializeSlot()
    {
        Transform tabTrans = transform.Find("EquipTab");
        if (tab.Equals((int)slotTab.Equip))
        {
            tabTrans.GetComponent<Image>().color = Color.red;
        }
        else
        {
            tabTrans.GetComponent<Image>().color = Color.white;
        }

        tabTrans = transform.Find("ConsumeTab");
        if (tab.Equals((int)slotTab.Consume))
        {
            tabTrans.GetComponent<Image>().color = Color.red;
        }
        else
        {
            tabTrans.GetComponent<Image>().color = Color.white;
        }

        tabTrans = transform.Find("EtcTab");
        if (tab.Equals((int)slotTab.Etc))
        {
            tabTrans.GetComponent<Image>().color = Color.red;
        }
        else
        {
            tabTrans.GetComponent<Image>().color = Color.white;
        }


        itemCnt = GameManager.Instance.nowPlayer.inventory.items.Count;
        int slotIndex = 0;
        // 보유 아이템 슬롯 할당
        for (int i = 0; i < itemCnt; i++)
        {
            Item item = GameManager.Instance.nowPlayer.inventory.items[i];
            if ((int)item.itemType != tab)
            {
                continue;
            }
            slot[slotIndex].icon.sprite = GameManager.Instance.nowPlayer.inventory.items[i].itemIcon;
            slot[slotIndex].icon.color = new Color(1, 1, 1, 1);
            if (GameManager.Instance.nowPlayer.inventory.items[i].quantity > 1)
            {
                slot[slotIndex].itemCount_Text.text = "x" + GameManager.Instance.nowPlayer.inventory.items[i].quantity.ToString();
            }
            else
            {
                slot[slotIndex].itemCount_Text.text = null;
            }
            slotIndex++;
        }
        // 미보유 아이템 초기화
        // 10번째 슬롯에 아이콘이 있는데, 아이템 목록이 9개이하일 경우 10번째 슬롯에 아이콘이 남아있는 것 방지
        for (int i = slotIndex; i < slotCnt; i++)
        {
            slot[i].icon.sprite = null;
            slot[i].icon.color = new Color(1, 1, 1, 0);
            slot[i].itemCount_Text.text = null;
        }
        if (moneyText == null)
        {
            transform.Find("Gold").TryGetComponent(out moneyText);
        }
        else
        {
            moneyText.text = GameManager.Instance.nowPlayer.Gold.ToString();
        }
    }

    public void EquipSlot()
    {
        tab = (int)slotTab.Equip;
        InitializeSlot();
    }
    public void ConsumeSlot()
    {
        tab = (int)slotTab.Consume;
        InitializeSlot();
    }
    public void EtcSlot()
    {
        tab = (int)slotTab.Etc;
        InitializeSlot();
    }
}
