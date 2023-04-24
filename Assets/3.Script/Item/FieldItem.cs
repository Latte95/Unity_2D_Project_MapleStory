using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    private Rigidbody2D rigid;
    private RectTransform childRectTransform;

    private float jumpForce = 5f;
    private float moveSpeed = 3f;
    private float moveDistance = 0.05f;

    public int money;

    private void OnEnable()
    {
        TryGetComponent(out rigid);
        Jump();
        childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        childRectTransform.localPosition = moveDistance * Vector3.up;
        // 일정 시간 안 먹으면 아이템 사라짐
        StartCoroutine(nameof(DestroyItem_co));
    }

    // 위아래 움직임
    private void Update()
    {
        float newY = moveDistance + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        childRectTransform.localPosition = new Vector3(childRectTransform.localPosition.x, newY, 0);
    }

    // 처음 나올 때
    public void Jump()
    {
        rigid.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }

    // 먹을 때 사라짐
    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    private IEnumerator DestroyItem_co()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
