using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public RectTransform transform_cursor;
    public Sprite defaultCursorImage; // 기본 커서 이미지
    public Sprite clickedCursorImage;
    public Image cursorImage;

    private void Awake()
    {
        Init_Cursor();
        transform_cursor.TryGetComponent(out cursorImage);
    }

    private void Update()
    {
        Update_MousePosition();
        //if (Input.GetMouseButtonDown(0))
        //{
        //    cursorImage.sprite = cursorImage.sprite.Equals(clickedCursorImage) ? defaultCursorImage : clickedCursorImage;
        //}
    }

    public void Init_Cursor()
    {
        //Cursor.visible = false;

        GameObject cursorGameObject = GameManager.UI.transform.Find("Cursor").gameObject;

        if (cursorGameObject != null)
        {
            // Assign the RectTransform component of the Cursor game object to transform_cursor
            transform_cursor = cursorGameObject.GetComponent<RectTransform>();
            transform_cursor.pivot = Vector2.up;

            if (transform_cursor.GetComponent<Graphic>())
            {
                transform_cursor.GetComponent<Graphic>().raycastTarget = false;
            }
            transform_cursor.TryGetComponent(out cursorImage);
        }
        else
        {
        }

    }

    private void Update_MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        transform_cursor.position = mousePos;
    }
    public void SetCursorImage(Sprite newImage)
    {
        cursorImage.sprite = newImage;
    }
}
