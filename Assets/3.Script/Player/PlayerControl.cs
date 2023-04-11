using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : ControlManager
{
    protected WaitForSeconds ignorePlatTime_wait;

    private void OnEnable()
    {
        ignorePlatTime_wait = new WaitForSeconds(1f);
    }

    protected override void Attack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        //StartCoroutine(nameof(CheckWall_co));
        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����, �ǰ����� �� �̵� �Ұ���
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(MoveAni)) &&
            !isImmobile)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.MoveTo(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.MoveTo(Vector3.right);
            }
            // �̵��� Ű ���� ����
            else
            {
                movement.MoveTo(Vector3.zero);
            }
        }
        // �����߿��� �̵� �Ұ���
        // => �������� �ƴҶ��� �̵� ����
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(Vector3.zero);
        }
    }

    protected override void GroundAct()
    {
        if (isGrounded)
        {
            Prone();
            if (Input.GetButtonDown("Jump"))
            {
                // ������ �� ����
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
                {
                    movement.JumpTo();
                }
                // ���� ���� �� ���� -> �Ʒ� ����
                else
                {
                    StartCoroutine(nameof(DownJump_co));
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            OnDamaged(col.gameObject.transform.position);
        }
    }


    IEnumerator Attack_co()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("MoveAni") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            anim.SetBool("isAttack", true);
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("isAttack", false);
        }
    }
        
    IEnumerator DownJump_co()
    {
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
    }


    private void Prone()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("isDown", true);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            anim.SetBool("isDown", false);
        }
    }
}
