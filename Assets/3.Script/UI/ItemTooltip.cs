using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    private RectTransform transform_cursor;
    public Image icon;
    public Text ItemName_Text;
    public Text StatNum_Text;
    public Text Description_Text;
    public CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
    }

    private void Update()
    {
        transform.position = Input.mousePosition + 80*Vector3.down + 65*Vector3.right;
    }

    public void SetUpTootip(int itemID)
    {
        int itemIndex = DataManager.instance.itemDataBase.itemList.FindIndex(item => item._itemID.Equals(itemID));
        Item tmpItem = DataManager.instance.itemDataBase.itemList[itemIndex];

        icon.sprite = tmpItem.itemIcon;
        ItemName_Text.text = tmpItem._itemName;
        StatNum_Text.text = $"{tmpItem._str} \n{tmpItem._dex} \n{tmpItem._int} \n{tmpItem._luk}";
        Description_Text.text = tmpItem._itemDescription;
    }
}
