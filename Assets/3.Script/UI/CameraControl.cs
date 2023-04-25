using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // �÷��̾���� ��ġ ����
    private Vector3 delta;

    private void Awake()
    {
        delta = 10 * Vector3.back + 0.01f * Vector3.up;
    }

    // �÷��̾� �̵� �� ī�޶� ����
    private void LateUpdate()
    {
        if (GameManager.Instance.nowPlayer != null)
        {
            transform.position = (Vector3)Vector2.Lerp(transform.position, GameManager.Instance.nowPlayer.transform.position, 0.01f) + delta;
        }
    }
}