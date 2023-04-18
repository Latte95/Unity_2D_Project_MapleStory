using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EquipableItem : Item
{
    // 장비 슬롯 부위
    public enum EQUIPTYPE
    {
        HEAD,
        BODY,
        WEAPON,
        FOOT,
    }

    public EQUIPTYPE equipType { get; set; }
    public EquipableItem(int _itemID, string _name, string _itemDescription, int _atk, int _def, int _str, int _dex, int _int, int _luk, int _hp, int _mp, int _price,
        ItemType _itemType, EQUIPTYPE _equipType, int _quantity = 1) :
        base(_itemID, _name, _itemDescription, _atk, _def, _str, _dex, _int, _luk, _hp, _mp, _price, _itemType, _quantity = 1)
    {
        this.equipType = _equipType;
    }
}
