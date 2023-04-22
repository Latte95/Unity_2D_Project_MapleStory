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
    Etc
}

public class MainUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject StatusUI;
    bool activeInventory = false;
    bool activeStatus = false;

    private void Start()
    {
        inventoryUI.SetActive(activeInventory);
        StatusUI.SetActive(activeStatus);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryUI.SetActive(activeInventory);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            activeStatus = !activeStatus;
            StatusUI.SetActive(activeStatus);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.SetActive(false);
            StatusUI.SetActive(false);
        }
    }
}
