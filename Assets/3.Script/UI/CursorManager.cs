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
        // ���� ���۽� ������ ����. �߻� ������ �𸣰���...
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
            // ���콺 Ŀ�� �̹����� �ƴ� �� Ŭ���ϸ� �ٽ� Ŀ�� �̹����� �ǵ���
            if (!cursorImage.sprite.name.Equals("Cursor_nomal"))
            {
                SetCursorImage(Resources.Load<Sprite>("Cursor_nomal"));
            }
        }
    }

    // Ŀ�� �̹��� ����
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
