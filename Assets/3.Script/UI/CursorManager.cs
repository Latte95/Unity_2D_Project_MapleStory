using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private RectTransform transform_cursor;
    public Image cursorImage;

    private void Start()
    {
        // 게임 시작시 널참조 방지. 발생 이유는 모르겠음...
        if (GameManager.UI == null)
        {
            return;
        }
        Init_Cursor();
        transform_cursor.TryGetComponent(out cursorImage);
    }

    private void Update()
    {
        Update_MousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 커서 이미지가 아닐 때 클릭하면 다시 커서 이미지로 되돌림
            if (!cursorImage.sprite.name.Equals("Cursor_nomal"))
            {
                SetCursorImage(Resources.Load<Sprite>("Cursor_nomal"));
            }
        }
    }

    // 커서 이미지 설정
    public void Init_Cursor()
    {
        Cursor.visible = false;
        GameObject cursorGameObject = GameManager.UI.transform.Find("Cursor").gameObject;

        if (cursorGameObject != null)
        {
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
        yield return new WaitForSeconds(0.15f);
        cursorImage.sprite = newImage;
    }
}
