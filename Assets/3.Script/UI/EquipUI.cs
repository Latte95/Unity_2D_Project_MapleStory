using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    public enum equipSlot
    {
        Clothes,
        Weapon,
        Pants,
        Cnt,
    }
    Player player;
    EquipSlot[] equipSlotReferences = new EquipSlot[(int)equipSlot.Cnt];
    EquipSlot equipSlotReference;

    void Start()
    {
        //player = GameManager.Instance.nowPlayer;

        //for (int i = 0; i < (int)equipSlot.Cnt; i++)
        //{
        //    equipSlotReferences[i] = transform.Find(((equipSlot)i).ToString()).GetComponent<EquipSlot>();
        //}
        //LoadData(player);
    }

    private void OnEnable()
    {
        if (player == null)
        {
            player = GameManager.Instance.nowPlayer;
            for (int i = 0; i < (int)equipSlot.Cnt; i++)
            {
                equipSlotReferences[i] = transform.Find(((equipSlot)i).ToString()).GetComponent<EquipSlot>();
            }
        }

        LoadData(player);
    }
    private void OnDisable()
    {
        SaveData(ref player);
    }

    public void LoadData(Player data)
    {
        for (int i = 0; i < (int)equipSlot.Cnt; i++)
        {
            Image iconImage = equipSlotReferences[i].transform.GetComponent<Image>();

            string iconID = data.equipSlot[i].ToString();
            if (data.equipSlot[i] != 0)
            {
                iconImage.sprite = Resources.Load<Sprite>("ItemIcon/" + iconID);
                iconImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                iconImage.color = new Color(1, 1, 1, 0);
                iconImage.sprite = null;
            }
        }
    }

    public void SaveData(ref Player data)
    {
        for (int i = 0; i < (int)equipSlot.Cnt; i++)
        {
            Image iconImage = equipSlotReferences[i].transform.GetComponent<Image>();

            if (iconImage.sprite != null)
            {
                data.equipSlot[i] = int.Parse(iconImage.sprite.name);
            }
            else
            {
                data.equipSlot[i] = 0;
            }
        }
    }
}
