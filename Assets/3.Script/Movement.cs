using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    private float boxCastMaxDistance = 0.7f;

    [SerializeField]
    private float speed = 0.3f;
    [SerializeField]
    private float jumpForce = 20f;
    private Vector3 moveDirection = Vector3.zero;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out spriteRenderer);
        deltaTime = Time.deltaTime;
    }

    void FixedUpdate()
    {
        rigid.position += speed * deltaTime * (Vector2)moveDirection;
        if (moveDirection.Equals(Vector3.left))
        {
            anim.SetInteger("h", -1);
            spriteRenderer.flipX = false;
        }
        else if (moveDirection.Equals(Vector3.right))
        {
            anim.SetInteger("h", 1);
            spriteRenderer.flipX = true;
        }
        else
        {
            anim.SetInteger("h", 0);
        }
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void JumpTo()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(jumpForce * Vector3.up, ForceMode2D.Impulse);
    }
}
