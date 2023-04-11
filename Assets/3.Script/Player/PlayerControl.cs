using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : ControlManager
{
    // �Ʒ������� �ٴ� �浹 ������ �ð�
    protected WaitForSeconds ignorePlatTime_wait;

    private void OnEnable()
    {
        ignorePlatTime_wait = new WaitForSeconds(0.35f);
    }

    // ����
    protected override void Attack()
    {
        // �⺻ ����
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����, �ǰ����� �� �̵� �Ұ���
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(moveAni)) &&
            !isImmobile)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.MoveTo(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.MoveTo(Vector2.right);
            }
            // �̵��� Ű ���� ����
            else
            {
                movement.MoveTo(Vector2.zero);
            }
        }
        // �����߿��� �̵� �Ұ���
        // => �������� �ƴҶ��� �̵� ����
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(Vector2.zero);//����
        }
    }

    protected override void GroundAct()
    {
        if (isGrounded)
        {
            // ���帮���� Ȯ��
            Prone();
            if (Input.GetButtonDown("Jump"))
            {
                // ������ �� ����
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
                {
                    movement.JumpTo();
                }
                // ���� ���� �� �Ʒ� ����
                else
                {
                    StartCoroutine(nameof(DownJump_co));
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // ���� �ε��� ��� �ǰݴ���
        if (col.gameObject.CompareTag("Enemy"))
        {
            OnDamaged(col.gameObject.transform.position);
        }
    }

    private void Prone()
    {
        // ���̱� �ִϸ��̼�
        if (Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("isDown", true);
        }
        if (!Input.GetKey(KeyCode.DownArrow) || !isGrounded)
        {
            anim.SetBool("isDown", false);
        }
    }


    IEnumerator Attack_co()
    {
        // ���� ������ ������ ���� ���� ����
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(moveAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            anim.SetBool("isAttack", true);
            yield return null; // ����
            anim.SetBool("isAttack", false);
        }
    }
        
    IEnumerator DownJump_co()
    {
        // ���� �ð����� �浹 ������
        // ���� �� �ٴ��� ��� Platform Effector�� UseCollider Mask üũ�Ͽ� �Ʒ����� ����
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
    }
}
