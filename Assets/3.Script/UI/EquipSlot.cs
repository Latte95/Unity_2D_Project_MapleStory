using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public Image icon;
    public CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>(); // Find the CursorManager in the scene
        icon = transform.GetComponent<Image>();
    }


    public void OnSlotClicked()
    {
        // �� ������ ����
        if (cursorManager.cursorImage.sprite.name[0].Equals('1'))
        {
            int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID == int.Parse(cursorManager.cursorImage.sprite.name));
            EquipableItem tmpItem = (EquipableItem)DataManager.instance.itemDataBase.itemList[itemIndex];
            if (gameObject.name.Equals(tmpItem.equipType.ToString()))
            {
                // ������ ��� ������ �κ��丮�� �ű�� ���� ����
                if (icon.sprite != null)
                {
                    GameManager.Instance.nowPlayer.inventory.GetItem(int.Parse(icon.sprite.name));
                    DifStat(int.Parse(icon.sprite.name));
                }
                // ���ο� ��� ����
                icon.sprite = cursorManager.cursorImage.sprite;
                icon.color = new Color(1, 1, 1, 1);
                GameManager.Instance.nowPlayer.inventory.RemoveItem(tmpItem);
                SumStat(tmpItem);
            }
        }
        // ������ ����
        else
        {
            if (icon.sprite != null)
            {
                GameManager.Instance.nowPlayer.inventory.GetItem(int.Parse(icon.sprite.name));
                DifStat(int.Parse(icon.sprite.name));
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0);
            }
        }
    }

    private void SumStat(Item tmpItem)
    {
        if (tmpItem._atk != 0)
        {
            GameManager.Instance.nowPlayer.Atk += tmpItem._atk;
        }
        if (tmpItem._def != 0)
        {
            GameManager.Instance.nowPlayer.Def += tmpItem._def;
        }
        if (tmpItem._str != 0)
        {
            GameManager.Instance.nowPlayer.Str += tmpItem._str;
        }
        if (tmpItem._dex != 0)
        {
            GameManager.Instance.nowPlayer.Dex += tmpItem._dex;
        }
        if (tmpItem._int != 0)
        {
            GameManager.Instance.nowPlayer.Int += tmpItem._int;
        }
        if (tmpItem._luk != 0)
        {
            GameManager.Instance.nowPlayer.Luk += tmpItem._luk;
        }
        if (tmpItem._hp != 0)
        {
            GameManager.Instance.nowPlayer.MaxHp += tmpItem._hp;
            GameManager.Instance.nowPlayer.Hp += tmpItem._hp;
        }
        if (tmpItem._mp != 0)
        {
            GameManager.Instance.nowPlayer.MaxMp += tmpItem._mp;
            GameManager.Instance.nowPlayer.Mp += tmpItem._mp;
        }
    }
    private void DifStat(int itemID)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID == itemID);
        EquipableItem tmpItem = (EquipableItem)DataManager.instance.itemDataBase.itemList[itemIndex];

        if (tmpItem._atk != 0)
        {
            GameManager.Instance.nowPlayer.Atk -= tmpItem._atk;
        }
        if (tmpItem._def != 0)
        {
            GameManager.Instance.nowPlayer.Def -= tmpItem._def;
        }
        if (tmpItem._str != 0)
        {
            GameManager.Instance.nowPlayer.Str -= tmpItem._str;
        }
        if (tmpItem._dex != 0)
        {
            GameManager.Instance.nowPlayer.Dex -= tmpItem._dex;
        }
        if (tmpItem._int != 0)
        {
            GameManager.Instance.nowPlayer.Int -= tmpItem._int;
        }
        if (tmpItem._luk != 0)
        {
            GameManager.Instance.nowPlayer.Luk -= tmpItem._luk;
        }
        if (tmpItem._hp != 0)
        {
            GameManager.Instance.nowPlayer.MaxHp -= tmpItem._hp;
        }
        if (tmpItem._mp != 0)
        {
            GameManager.Instance.nowPlayer.MaxMp -= tmpItem._mp;
        }

    }
}
