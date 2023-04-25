using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text itemCount_Text;
    public CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        icon = transform.Find("Image").GetComponent<Image>();
    }


    public void OnSlotClicked()
    {        
        if (icon.sprite != null)
        {
            cursorManager.SetCursorImage(icon.sprite);
        }
    }
}