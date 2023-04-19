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
        cursorManager = FindObjectOfType<CursorManager>(); // Find the CursorManager in the scene
        icon = transform.Find("Image").GetComponent<Image>();
    }


    public void OnSlotClicked()
    {        
        if (icon.sprite != null) // Check if there's an item in the slot
        {
            cursorManager.SetCursorImage(icon.sprite); // Set the cursor image to the item's icon
        }
    }
}
