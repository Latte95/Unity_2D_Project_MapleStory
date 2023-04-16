using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ConsumableItem : Item
{
    // 소비아이템
    public ConsumableItem(int _itemID, string _name, string _itemDescription, int _atk, int _def, int _str, int _dex, int _int, int _luk, int _hp, int _mp, int _price, ItemType _itemType, int _quantity = 1) : base(_itemID, _name, _itemDescription, _atk, _def, _str, _dex, _int, _luk, _hp, _mp, _price, _itemType, _quantity = 1)
    {
        this.quantity = quantity;
    }
}
