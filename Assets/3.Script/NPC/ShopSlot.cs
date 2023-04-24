using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon;
    public Text itemName;
    public Text itemPrice;
    public Text itemQuantity;
    public CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
    }

    public void OnSlotClicked()
    {
        if (icon.sprite != null)
        {
            cursorManager.SetCursorImage(icon.sprite);
        }
    }
}
