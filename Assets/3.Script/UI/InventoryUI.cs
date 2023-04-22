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
    private Transform SlotTrans;
    [SerializeField]
    private GameObject SlotPrefabs;

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
    }

    private void OnEnable()
    {
        InitializeSlot();
        // �κ��丮 ���� ���·� ������ �߰��� ���Կ� �ٷιٷ� ä����
        inventory.OnItemAdded += UpdateUI;
        // �޼� ȹ�� ����
        if (GameManager.Instance.player != null)
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded += InitializeSlot;
        }
    }
    private void OnDisable()
    {
        inventory.OnItemAdded -= UpdateUI;
        GameManager.Instance.player.GetComponent<PlayerControl>().OnMoneyAdded -= InitializeSlot;
    }

    private void Start()
    {
        inventory = GameManager.Instance.nowPlayer.inventory;
        itemCnt = GameManager.Instance.nowPlayer.inventory.items.Count;
        // �κ��丮 ����, �κ��丮 ���� ���·� �߰��� �������� �����ϱ� ����
        InitializeSlot();
        inventory.OnItemAdded += UpdateUI;
    }


    private void InitializeSlot()
    {
        itemCnt = GameManager.Instance.nowPlayer.inventory.items.Count;
        // ���� ������ ���� �Ҵ�
        for (int i = 0; i < itemCnt; i++)
        {
            slot[i].icon.sprite = GameManager.Instance.nowPlayer.inventory.items[i].itemIcon;
            slot[i].icon.color = new Color(1, 1, 1, 1);
            if (GameManager.Instance.nowPlayer.inventory.items[i].quantity > 1)
            {
                slot[i].itemCount_Text.text = "x" + GameManager.Instance.nowPlayer.inventory.items[i].quantity.ToString();
            }
            else
            {
                slot[i].itemCount_Text.text = null;
            }
        }
        // �̺��� ������ �ʱ�ȭ
        // 10��° ���Կ� �������� �ִµ�, ������ ����� 9�������� ��� 10��° ���Կ� �������� �����ִ� �� ����
        for (int i = itemCnt; i < slotCnt; i++)
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

    private void UpdateUI(Item newItem)
    {
        if (newItem == null)
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
            // �� ������ ȹ��� ���� ������
            if (!slot[itemIndex].icon.sprite.name.Equals(newItem.itemIcon.name))
            {
                InitializeSlot();
                return;
            }
            // �̹� �ִ� �������̸� ������ ����
            slot[itemIndex].icon.sprite = newItem.itemIcon;
            slot[itemIndex].icon.color = new Color(1, 1, 1, 1);
            if (GameManager.Instance.nowPlayer.inventory.items[itemIndex].quantity > 1)
            {
                slot[itemIndex].itemCount_Text.text = "x" + GameManager.Instance.nowPlayer.inventory.items[itemIndex].quantity.ToString();
            }
            else if (GameManager.Instance.nowPlayer.inventory.items[itemIndex].quantity.Equals(1))
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
        // ���������̸� 
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
                    break;
                }
            }
        }
    }
}
