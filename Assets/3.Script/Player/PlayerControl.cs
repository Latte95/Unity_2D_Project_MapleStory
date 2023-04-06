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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Nomal01"))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.MoveTo(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.MoveTo(Vector3.right);
            }
            else if (!Input.anyKey)
            {
                movement.MoveTo(Vector3.zero);
            }
        }
        if (Input.GetButtonDown("Jump") && IsOnGround())
        {
            movement.JumpTo();
        }
        IsOnGround();

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Attack_co());
        }
    }

    IEnumerator Attack_co()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || 
            anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Nomal") || 
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            anim.SetBool("isAttack", true);
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("isAttack", false);
        }
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
