using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private RectTransform transform_cursor;
    public Sprite defaultCursorImage; // 기본 커서 이미지
    public Sprite clickedCursorImage;
    public Image cursorImage;

    private WaitForSeconds waitShort_wait;

    private void Start()
    {
        // 씬 전환시 널참조 방지. 발생 이유는 모르겠음...
        if (GameManager.UI == null)
        {
            return;
        }
        Init_Cursor();
        transform_cursor.TryGetComponent(out cursorImage);
        waitShort_wait = new WaitForSeconds(0.01f);
    }

    private void Update()
    {
        Update_MousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            if (!cursorImage.sprite.name.Equals("Cursor_nomal"))
            {
                SetCursorImage(Resources.Load<Sprite>("Cursor_nomal"));
            }
        }
    }

    public void Init_Cursor()
    {
        Cursor.visible = false;
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
    }

    private void Update_MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        if (transform_cursor != null)
        {
            transform_cursor.position = mousePos;
        }
    }
    public void SetCursorImage(Sprite newImage)
    {
        StopCoroutine(SetCursorImage_co(newImage));
        StartCoroutine(SetCursorImage_co(newImage));
    }

    IEnumerator SetCursorImage_co(Sprite newImage)
    {
        yield return waitShort_wait;
        cursorImage.sprite = newImage;
    }
}
