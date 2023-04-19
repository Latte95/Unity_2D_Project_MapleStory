using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    // �÷��̾���� ��ġ ����
    Vector3 delta;

    [SerializeField]
    GameObject player;

    private void Start()
    {
        InitializePlayer();
        if (player != null)
        {
            transform.position = (Vector2)player.transform.position;
        }
    }

    // �÷��̾� �̵� �� ī�޶� ����
    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = (Vector3)Vector2.Lerp(transform.position, player.transform.position, 0.01f) + delta;
        }
    }

    public void InitializePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player object not found in the scene.");
        }
    }
}
