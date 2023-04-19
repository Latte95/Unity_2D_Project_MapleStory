using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    public Image icon;
    public CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        icon = transform.Find("Icon").GetComponent<Image>();
    }    

    public void OnSlotClicked()
    {
        if (cursorManager.cursorImage.sprite.name[0] == '2')
        {
            icon.sprite = cursorManager.cursorImage.sprite;
            icon.color = new Color(1, 1, 1, 1);
        }
    }
}
