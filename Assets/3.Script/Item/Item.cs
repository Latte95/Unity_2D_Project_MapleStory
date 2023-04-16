using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    public enum ItemType
    {
        Equip,
        Consume,
        Etc,
    }

    // 소비 아이템 최대 보유 가능 개수
    const int MAX_CONSUMABLE_NUMBER = 99;

    // 장비랑 소비 아이템 공통 특성
    public int _itemID;
    public string _itemName;
    public string _itemDescription;
    public int _atk;
    public int _def;
    public int _str;
    public int _dex;
    public int _int;
    public int _luk;
    public int _hp;
    public int _mp;
    public int _price;
    private int _quantity;
    public int quantity
    {
        get { return _quantity; }
        // 보유 개수 제한
        set
        {
            if (value > MAX_CONSUMABLE_NUMBER)
            {
                _quantity = MAX_CONSUMABLE_NUMBER;
            }
            _quantity = value;
        }
    }
    public Sprite itemIcon;
    public ItemType itemType;
    //public GameObject itemPrefab;


    // 공통 특성 초기화
    public Item(int _itemID, string _name, string _itemDescription, int _atk, int _def, int _str, int _dex, int _int, int _luk, int _hp, int _mp, int _price, ItemType _itemType, int _quantity = 1)
    {
        this._itemID = _itemID;
        this._itemName = _name;
        this._itemDescription = _itemDescription;
        this._atk = _atk;
        this._def = _def;
        this._str = _str;
        this._dex = _dex;
        this._int = _int;
        this._luk = _luk;
        this._hp = _hp;
        this._mp = _mp;
        this._price = _price;
        this.itemType = _itemType;
        this._quantity = _quantity;
        itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite)) as Sprite;
    }
}
