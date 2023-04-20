using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    // �÷��̾���� ��ġ ����
    Vector3 delta;

    // �÷��̾� �̵� �� ī�޶� ����
    private void LateUpdate()
    {
        if (GameManager.Instance.nowPlayer != null)
        {
            transform.position = (Vector3)Vector2.Lerp(transform.position, GameManager.Instance.nowPlayer.transform.position, 0.01f) + delta;
        }
    }
}
