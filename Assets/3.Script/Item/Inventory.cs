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

    // 인벤토리에 아이템 추가 (구매, 착용, 드랍 등등)
    public void AddItem(Item item)
    {
        if (item is ConsumableItem)
        {
            // 같은 이름을 가진 아이템이 있는지 찾기
            int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

            // 아이템이 있으면 수량증가
            if (itemIndex >= 0)
            {
                items[itemIndex].quantity++;
                //return;
            }
            // 아이템이 없으면 인벤토리에 추가
            else
            {
                items.Add(item);
                item.quantity = 1;
            }
        }
        // 장비 아이템을 추가할 땐
        else if (item is EquipableItem)
        {
            // 그냥 아이템 추가
            items.Add(item);
            item.quantity = 1;

        }
        else
        {
            // 기타 아이템
        }
        UpdateSlot(item);
    }

    // 아이템 삭제 (판매, 해제, 버림, 사용 등등)
    public void RemoveItem(Item item)
    {
        // 같은 이름을 가진 아이템이 있는지 찾기
        int itemIndex = items.FindIndex(invenItem => invenItem._itemName == item._itemName);

        // 인벤토리에 삭제할 아이템이 있다면
        if (itemIndex >= 0)
        {
            // 소비 아이템
            if (item is ConsumableItem)
            {
                // 보유 하고 있으면 1개씩 제거하고
                if (items[itemIndex].quantity > 0)
                {
                    items[itemIndex].quantity--;
                }
                // 수량이 0이 되면 완전히 제거
                if (items[itemIndex].quantity <= 0)
                {
                    items.RemoveAt(itemIndex);
                }
            }
            // 장비 아이템은 그냥 완전히 제거
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
