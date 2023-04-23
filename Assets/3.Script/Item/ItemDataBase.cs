using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase
{
    public List<Item> itemList = new List<Item>();

    public ItemDataBase(){
        itemList.Add(new ConsumableItem(02000000, "빨간 포션", "HP를 50 증가시킨다", 0, 0, 0, 0, 0, 0, 50, 0, 10, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000001, "주황 포션", "HP를 100 증가시킨다", 0, 0, 0, 0, 0, 0, 100, 0, 18, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000003, "파란 포션", "MP를 50 증가시킨다", 0, 0, 0, 0, 0, 0, 0, 50, 20, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000005, "엘릭서", "HP, MP를 50 증가시킨다", 0, 0, 0, 0, 0, 0, 50, 50, 25, Item.ItemType.Consume));
        itemList.Add(new EquipableItem(01302000, "검", "한손검", 0, 0, 5, 0, 0, 0, 0, 0, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.Weapon));
        itemList.Add(new EquipableItem(01302007, "카알 대검", "한손검", 0, 0, 8, 0, 0, 0, 0, 0, 300, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.Weapon));
        itemList.Add(new EquipableItem(01040002, "하얀 반팔 면티", "하얀 반팔 면티", 0, 5, 0, 0, 0, 0, 0, 0, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.Clothes));
        itemList.Add(new EquipableItem(1060002, "파란 청 바지", "파란 청 바지", 0, 3, 0, 0, 0, 0, 0, 10, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.Pants));
        itemList.Add(new Item(04000000, "파란 달팽이의 껍질", "파란 달팽이의 껍질을 벗긴 것이다.", 0, 0, 0, 0, 0, 0, 0, 0, 50, Item.ItemType.Etc));
        itemList.Add(new Item(04000001, "주황버섯의 갓", "주황버섯의 갓을 자른 것이다.", 0, 0, 0, 0, 0, 0, 0, 0, 300, Item.ItemType.Etc));
        itemList.Add(new Item(04000003, "나뭇가지", "나무의 가지를 꺾어온 것이다.", 0, 0, 0, 0, 0, 0, 0, 0, 100, Item.ItemType.Etc));
        itemList.Add(new Item(04000004, "물컹물컹한 액체", "점성이 높아 끈적끈적한 액체이다.", 0, 0, 0, 0, 0, 0, 0, 0, 150, Item.ItemType.Etc));
        itemList.Add(new Item(04000012, "초록버섯의 갓", "초록버섯의 갓을 자른 것이다.", 0, 0, 0, 0, 0, 0, 0, 0, 700, Item.ItemType.Etc));
    }
}
