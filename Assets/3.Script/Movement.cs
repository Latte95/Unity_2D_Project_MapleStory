using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rigid;

    [SerializeField]
    private float speed = 0.3f;
    [SerializeField]
    private float jumpForce = 20f;
    private Vector3 moveDirection = Vector3.zero;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out rigid);
        deltaTime = Time.deltaTime;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * moveDirection;
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void JumpTo()
    {
        rigid.AddForce(jumpForce * Vector3.up, ForceMode2D.Impulse);
    }
}
