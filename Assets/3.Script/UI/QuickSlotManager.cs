using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
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
    }
    Player player;
    QuickSlot[] quickSlotReferences = new QuickSlot[8];
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
        player = FindObjectOfType<Player>();
        Transform childObject = transform.GetChild(0);

        Transform grandchildObject = childObject.Find("Shift");
        quickSlotReferences[0] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Ins");
        quickSlotReferences[1] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Hm");
        quickSlotReferences[2] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Pup");
        quickSlotReferences[3] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Ctrl");
        quickSlotReferences[4] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Del");
        quickSlotReferences[5] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("End");
        quickSlotReferences[6] = grandchildObject.GetComponent<QuickSlot>();
        grandchildObject = childObject.Find("Pdn");
        quickSlotReferences[7] = grandchildObject.GetComponent<QuickSlot>();
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
            quickSlotReference = quickSlotReferences[(int)quickSlot.Shift];            
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
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{


        //}
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
                        itemUsePlayer.PlayOneShot(itemUseClip);
                        Item tmpItem = player.inventory.items[itemIndex];
                        player.Hp += tmpItem._hp;
                        player.inventory.RemoveItem(tmpItem);
                    }
                }
            }
        }
    }
}
