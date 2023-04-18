using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    private int slotCnt;

    private void Start()
    {
        slot = new InventorySlot[40];
        inventory = player.inventory;
        slotCnt = inventory.items.Count;
        StartCoroutine(nameof(UpdateSlot_co));
    }

    IEnumerator UpdateSlot_co()
    {
        yield return new WaitUntil(() => slotCnt != inventory.items.Count);
        slotCnt = inventory.items.Count;
        for (int i = 0; i < slotCnt; i++)
        {
            if (slot[i] == null)
            {
                GameObject slotClone = Instantiate(SlotPrefabs);
                slotClone.transform.SetParent(SlotTrans, false);

                InventorySlot newSlot = slotClone.GetComponent<InventorySlot>();
                newSlot.icon.sprite = inventory.items[i].itemIcon;
                // 아이템 개수가 있다면
                if (inventory.items[i].quantity > 0)
                {
                    newSlot.itemCount_Text.text = inventory.items[i].quantity.ToString();
                }

                slot[i] = newSlot;
            }
        }
    }

    public void AddSlot()
    {
        int length = inventory.items.Count;
        for (int i = 0; i < length; i++)
        {
            if (slot[i] == null)
            {
                GameObject slotClone = Instantiate(SlotPrefabs);
                slotClone.transform.SetParent(SlotTrans, false);

                InventorySlot newSlot = slotClone.GetComponent<InventorySlot>();
                newSlot.icon.sprite = inventory.items[i].itemIcon;
                // 아이템 개수가 있다면
                if (inventory.items[i].quantity > 0)
                {
                    newSlot.itemCount_Text.text = inventory.items[i].quantity.ToString();
                }

                slot[i] = newSlot;
            }
        }
    }
}
