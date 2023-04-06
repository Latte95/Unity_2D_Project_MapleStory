using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Movement movement;
    SpriteRenderer spriteRenderer;
    Animator anim;
    private Rigidbody2D rigid;

    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    private float boxCastMaxDistance = 0.7f;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out anim);
        TryGetComponent(out rigid);

        deltaTime = Time.deltaTime;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        movement.MoveTo(new Vector3(h, 0, 0));

        if (Input.GetButtonDown("Jump") && IsOnGround())
        {
            movement.JumpTo();
        }
        IsOnGround();

        //if (h > 0)
        //{
        //    spriteRenderer.flipX = true;
        //}
        //else if (h < 0)
        //{
        //    spriteRenderer.flipX = false;
        //}
        //if (h != anim.GetInteger("h"))
        //{
        //    anim.SetInteger("h", (int)h);
        //}
    }

    private bool IsOnGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down,
                                                    boxCastMaxDistance, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null)
        {
            anim.SetBool("isGrounded", true);
        }
        else
        {
            anim.SetBool("isGrounded", false);
        }
        return (raycastHit.collider != null);
    }

    void OnDrawGizmos()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));

        Gizmos.color = Color.red;
        if (raycastHit.collider != null)
        {
            Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * raycastHit.distance, boxCastSize);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.down * boxCastMaxDistance);
        }
    }

}
