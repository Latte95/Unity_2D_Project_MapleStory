using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private PlayerStat Stat;

    public Define.MoveDirection currentMoveDirection = Define.MoveDirection.None;

    // 아래점프시 바닥 충돌 무시할 시간
    protected WaitForSeconds ignorePlatTime_wait;


    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioJump);
        TryGetComponent(out Stat);
        Stat.Init();
        ignorePlatTime_wait = new WaitForSeconds(0.5f);
    }

    // 공격
    protected override void Attack()
    {
        // 기본 공격
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능
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
            // 엎드리는지 확인
            Prone();
            if (Input.GetButtonDown("Jump"))
            {
                PlaySound("Jump");
                // 서있을 때 점프
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
                {
                    movement.JumpTo();
                }
                // 누워 있을 때 아래 점프
                else
                {
                    StartCoroutine(nameof(DownJump_co));
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // 적과 부딪힌 경우 피격당함
        if (col.gameObject.CompareTag("Enemy"))
        {
            OnDamaged(col.gameObject.transform.position);
        }
    }

    private void Prone()
    {
        // 숙이기 애니메이션
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
        // 공격 가능한 상태일 때만 공격 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            anim.SetBool("isAttack", true);
            yield return null; // 수정
            anim.SetBool("isAttack", false);
        }
    }

    IEnumerator DownJump_co()
    {
        // 일정 시간동안 충돌 무시함
        // 가장 밑 바닥일 경우 Platform Effector의 UseCollider Mask 체크하여 아래점프 방지
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Debug.Log(1);
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
        //Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
    }
}
