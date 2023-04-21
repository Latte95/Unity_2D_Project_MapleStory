using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTNType
{
    Login,
    Yes,
    Back,
    Exit,
    NextScene,
    Dice,
    Play,
    Create,
    BackToFirst,
    Item,
}

public class MainUI : MonoBehaviour
{
    public GameObject inventoryUI;
    bool activeInventory = false;

    private void Start()
    {
        inventoryUI.SetActive(activeInventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryUI.SetActive(activeInventory);
        }
        if(Input.GetKeyDown(KeyCode.Escape) && activeInventory)
        {
            activeInventory = !activeInventory;
            inventoryUI.SetActive(activeInventory);
        }
    }
}
