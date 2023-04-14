using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    // 플레이어와의 위치 차이
    Vector3 delta;

    [SerializeField]
    GameObject player;

    private void Start()
    {
        if (player != null)
        {
            transform.position = (Vector2)player.transform.position;
        }
        else
        {
            transform.position = Vector2.zero;
        }
    }

    // 플레이어 이동 후 카메라 따라감
    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = (Vector3)Vector2.Lerp(transform.position, player.transform.position, 0.01f) + delta;
        }
    }
}
