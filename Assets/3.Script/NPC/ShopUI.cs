using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Text moneyText;
    GameObject canvas;
    public List<int> items;

    private void OnEnable()
    {
        if (canvas == null && GameObject.FindGameObjectWithTag("UI"))
        {
            canvas = GameObject.FindGameObjectWithTag("UI");
        }
        if (canvas != null)
        {
            canvas.transform.Find("InventoryUI").gameObject.SetActive(false);
            canvas.transform.Find("EquipUI").gameObject.SetActive(false);
            canvas.transform.Find("StatusUI").gameObject.SetActive(false);
            canvas.transform.Find("HUD").gameObject.SetActive(false);
        }
        Initialized();
    }

    private void OnDisable()
    {
        if(canvas != null)
        {
            canvas.transform.Find("HUD").gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialized()
    {
        if (moneyText == null)
        {
            transform.Find("Gold").TryGetComponent(out moneyText);
        }
        else
        {
            moneyText.text = GameManager.Instance.nowPlayer.Gold.ToString();
        }
    }
}
