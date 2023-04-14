using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Animator anim;
    private Stat Stat;

    private Vector2 moveDirection = Vector2.zero;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out Stat);

        deltaTime = Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // 이동 -> 리지드바디 이용 : 속도향상, 버벅임 없음
        //rigid.position += Stat.Speed * deltaTime * moveDirection;
        Debug.Log("이동 실행"); //수정
        rigid.velocity = new Vector3(Stat.Speed * moveDirection.x, rigid.velocity.y, 0);
        // 방향 설정
        if (moveDirection.Equals(Vector2.left))
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 0); ;
            }
        }
        else if (moveDirection.Equals(Vector2.right))
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 0); ;
            }
        }
    }


    public void MoveTo(float h)
    {
        switch (h)
        {
            case 1:
                moveDirection = Vector2.right;
                break;
            case 0:
                moveDirection = Vector2.zero;
                break;
            case -1:
                moveDirection = Vector2.left;
                break;
        }
        anim.SetInteger("h", (int)h);
    }

    public void JumpTo()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(Stat.JumpForce * Vector3.up, ForceMode2D.Impulse);
    }
}
