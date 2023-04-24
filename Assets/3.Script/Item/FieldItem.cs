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
        // ���� �ð� �� ������ ������ �����
        StartCoroutine(nameof(DestroyItem_co));
    }

    // ���Ʒ� ������
    private void Update()
    {
        float newY = moveDistance + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        childRectTransform.localPosition = new Vector3(childRectTransform.localPosition.x, newY, 0);
    }

    // ó�� ���� ��
    public void Jump()
    {
        rigid.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }

    // ���� �� �����
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
