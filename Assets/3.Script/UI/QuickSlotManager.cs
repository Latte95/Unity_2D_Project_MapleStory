using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotManager : MonoBehaviour, IDataPersistence
{
    public enum quickSlot
    {
        Shift,
        Ins,
        Hm,
        Pup,
        Ctrl,
        Del,
        End,
        Pdn,
        Cnt,
    }
    Player player;
    QuickSlot[] quickSlotReferences = new QuickSlot[(int)quickSlot.Cnt];
    QuickSlot quickSlotReference;

    [SerializeField]
    private AudioClip itemUseClip;
    AudioSource itemUsePlayer;

    private void OnEnable()
    {
        TryGetComponent(out itemUsePlayer);
        itemUsePlayer.playOnAwake = false;
    }

    private void Start()
    {
        player = GameManager.Instance.nowPlayer;
        Transform childObject = transform.GetChild(0);
        Transform grandchildObject = null;

        for(int i = 0; i<(int)quickSlot.Cnt;i++)
        {
        grandchildObject = childObject.Find(((quickSlot)i).ToString());
        quickSlotReferences[i] = grandchildObject.GetComponent<QuickSlot>();

        }

        LoadData(player);
    }

    private void Update()
    {
        if (quickSlotReference != null)
        {
            quickSlotReference = null;
        }
        // Äü½½·Ô
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().Magic();
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.Ins];
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.Hm];
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.Pup];
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameManager.Instance.player.GetComponent<PlayerControl>().Attack();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.Del];
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.End];
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            quickSlotReference = quickSlotReferences[(int)quickSlot.Pdn];
        }
        if (quickSlotReference != null)
        {
            if (quickSlotReference.icon.sprite != null)
            {
                if (quickSlotReference.icon.sprite.name[0].Equals('2'))
                {
                    int itemIndex = player.inventory.items.FindIndex(invenItem => invenItem._itemID == int.Parse(quickSlotReference.icon.sprite.name));

                    if (itemIndex >= 0)
                    {
                        Item tmpItem = player.inventory.items[itemIndex];
                        if ((tmpItem._hp > 0 && !player.Hp.Equals(player.MaxHp)) ||
                            (tmpItem._mp > 0 && !player.Mp.Equals(player.MaxMp)))
                        {
                            itemUsePlayer.PlayOneShot(itemUseClip);
                            player.Hp += tmpItem._hp;
                            player.Mp += tmpItem._mp;
                            player.inventory.RemoveItem(tmpItem);
                        }
                    }
                }
            }
        }
    }

    public void LoadData(Player data)
    {
        for (int i = 0; i < 8; i++)
        {
            Transform iconTransform = quickSlotReferences[i].transform.Find("Icon");
            Image iconImage = iconTransform.GetComponent<Image>();

            string iconID = data.quickSlot[i].ToString();
            if (data.quickSlot[i] != 0)
            {
                if (iconID[0].Equals('2'))
                {
                    iconImage.sprite = Resources.Load<Sprite>("ItemIcon/" + iconID);
                }
                else if (iconID[0].Equals('8'))
                {
                    iconImage.sprite = Resources.Load<Sprite>("SkillIcon/" + iconID);
                }
                iconImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                iconImage.sprite = null;
            }
        }
    }

    public void SaveData(ref Player data)
    {
        for (int i = 0; i < (int)quickSlot.Cnt; i++)
        {
            Transform iconTransform = quickSlotReferences[i].transform.Find("Icon");

            Image iconImage = iconTransform.GetComponent<Image>();

            if (iconImage.sprite != null)
            {
                data.quickSlot[i] = int.Parse(iconImage.sprite.name);
            }
            else
            {
                data.quickSlot[i] = 0;
            }
        }
    }
}
