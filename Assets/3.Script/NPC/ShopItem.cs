using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    Player player;
    int itemID;
    Text gold;

    private void OnEnable()
    {
        gold = GameObject.FindGameObjectWithTag("NPC UI").transform.Find("Shop").transform.Find("Gold").GetComponent<Text>();
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameManager.Instance.nowPlayer;
        }
    }

    public void BuyClick()
    {
        if (GetComponent<Image>().sprite != null)
        {
            int.TryParse(GetComponent<Image>().sprite.name, out itemID);
        }
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID.Equals(itemID));
        Item tmpItem = DataManager.instance.itemDataBase.itemList[itemIndex];

        if (player.Gold > tmpItem._price)
        {
            player.Gold -= tmpItem._price;
            player.inventory.GetItem(itemID);
        }
        gold.text = player.Gold.ToString();
    }
    public void SellClick()
    {
        int itemIndex = player.inventory.items.FindIndex(item => item._itemID.Equals(int.Parse(GetComponent<Image>().sprite.name)));
        Item tmpItem = player.inventory.items[itemIndex];

        player.Gold += (int)(tmpItem._price * 0.5f);
        player.inventory.RemoveItem(tmpItem);
        gold.text = player.Gold.ToString();
    }
}
