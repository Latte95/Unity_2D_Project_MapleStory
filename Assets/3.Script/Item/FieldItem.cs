using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    Rigidbody2D rigid;
    float jumpForce = 5f;

    RectTransform childRectTransform;
    float moveSpeed = 3f;
    float moveDistance = 0.05f;

    public int money;

    private void OnEnable()
    {
        TryGetComponent(out rigid);
        Jump();
        childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        childRectTransform.localPosition = moveDistance * Vector3.up;
    }

    private void Update()
    {
        float newY = moveDistance + Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        childRectTransform.localPosition = new Vector3(childRectTransform.localPosition.x, newY, 0);
    }

    public void Jump()
    {
        rigid.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }


    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
