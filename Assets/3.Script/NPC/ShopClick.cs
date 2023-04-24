using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NPC 클릭
public class ShopClick : MonoBehaviour
{
    private Transform shop;
    private bool isClickWaiting = false;

    WaitForSeconds doubleClick_wait;

    private void Awake()
    {
        doubleClick_wait = new WaitForSeconds(0.3f);
    }

    private void Start()
    {
        if (transform.parent.Find("Shop"))
        {
            shop = transform.parent.Find("Shop");
            shop.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // 더블클릭
            if (hit.collider != null && hit.collider.gameObject.Equals(gameObject))
            {
                if (!isClickWaiting)
                {
                    StartCoroutine(ClickDelay());
                }
                else
                {
                    OnClick();
                }
            }
        }
    }

    private IEnumerator ClickDelay()
    {
        isClickWaiting = true;
        yield return doubleClick_wait;
        isClickWaiting = false;
    }

    // 더블클릭하면 실행
    public void OnClick()
    {
        if (shop != null)
        {
            shop.gameObject.SetActive(true);
            shop.Find("NPC").GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            shop.GetComponent<ShopUI>().items = gameObject.GetComponent<ShopData>().itemID;
        }
    }
}
