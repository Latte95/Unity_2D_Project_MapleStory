using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Animator anim;

    [SerializeField]
    private float speed = 0.3f;
    [SerializeField]
    private float jumpForce = 20f;
    private Vector2 moveDirection = Vector2.zero;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);

        deltaTime = Time.deltaTime;
    }

    void FixedUpdate()
    {
        // 이동 -> 리지드바디 이용 : 속도향상, 버벅임 없음
        rigid.position += speed * deltaTime * moveDirection;
        // 방향 설정
        if (moveDirection.Equals(Vector2.left))
        {
            anim.SetInteger("h", -1);
            spriteRenderer.flipX = false;
        }
        else if (moveDirection.Equals(Vector2.right))
        {
            anim.SetInteger("h", 1);
            spriteRenderer.flipX = true;
        }
        else
        {
            anim.SetInteger("h", 0);
        }
    }


    public void MoveTo(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void JumpTo()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(jumpForce * Vector3.up, ForceMode2D.Impulse);
    }
}
