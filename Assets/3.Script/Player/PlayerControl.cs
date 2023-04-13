using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private PlayerStat Stat;

    public Define.MoveDirection currentMoveDirection = Define.MoveDirection.None;

    // �Ʒ������� �ٴ� �浹 ������ �ð�
    protected WaitForSeconds ignorePlatTime_wait;


    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioJump);
        TryGetComponent(out Stat);
        Stat.Init();
        ignorePlatTime_wait = new WaitForSeconds(0.5f);
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
        // ������ �ְų� �ȴ� �߿��� �̵� ����
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni)))
        {
            float h = Input.GetAxisRaw("Horizontal");
            movement.MoveTo(h);
            if (Input.GetButtonDown("Jump"))
            {
                movement.JumpTo();
            }
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
                PlaySound("Jump");
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

    protected override void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Hit":
                //audioSource.clip = audioHit;
                break;
        }
    }


    IEnumerator Attack_co()
    {
        // ���� ������ ������ ���� ���� ����
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
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
        Debug.Log(1);
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
        //Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
    }
}
