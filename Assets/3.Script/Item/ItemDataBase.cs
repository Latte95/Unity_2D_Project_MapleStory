using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase
{
    public List<Item> itemList = new List<Item>();

    public ItemDataBase(){
        itemList.Add(new ConsumableItem(02000000, "���� ����", "HP�� 50 ������Ų��", 0, 0, 0, 0, 0, 0, 50, 0, 10, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000001, "��Ȳ ����", "HP�� 100 ������Ų��", 0, 0, 0, 0, 0, 0, 100, 0, 18, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000003, "�Ķ� ����", "MP�� 50 ������Ų��", 0, 0, 0, 0, 0, 0, 0, 50, 20, Item.ItemType.Consume));
        itemList.Add(new ConsumableItem(02000004, "������", "HP, MP�� 50 ������Ų��", 0, 0, 0, 0, 0, 0, 50, 50, 25, Item.ItemType.Consume));
        itemList.Add(new EquipableItem(01302000, "��", "�Ѽհ�", 5, 0, 0, 0, 0, 0, 0, 0, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.WEAPON));
        itemList.Add(new EquipableItem(01040002, "�Ͼ� ���� ��Ƽ", "�Ͼ� ���� ��Ƽ", 0, 5, 0, 0, 0, 0, 0, 0, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.BODY));
        itemList.Add(new EquipableItem(01302000, "�Ķ� û ����", "�Ķ� û ����", 0, 3, 0, 0, 0, 0, 0, 20, 100, Item.ItemType.Equip, EquipableItem.EQUIPTYPE.FOOT));
        itemList.Add(new Item(04000000, "�Ķ� �������� ����", "�Ķ� �������� ������ ���� ���̴�.", 0, 0, 0, 0, 0, 0, 0, 0, 50, Item.ItemType.Etc));
        itemList.Add(new Item(04000001, "��Ȳ������ ��", "��Ȳ������ ���� �ڸ� ���̴�.", 0, 0, 0, 0, 0, 0, 0, 0, 500, Item.ItemType.Etc));
        itemList.Add(new Item(04000003, "��������", "������ ������ ����� ���̴�.", 0, 0, 0, 0, 0, 0, 0, 0, 100, Item.ItemType.Etc));
        itemList.Add(new Item(04000004, "���ȹ����� ��ü", "������ ���� ���������� ��ü�̴�.", 0, 0, 0, 0, 0, 0, 0, 0, 150, Item.ItemType.Etc));
    }
}
