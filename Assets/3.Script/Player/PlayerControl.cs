using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private Player Stat;

    public Define.MoveDirection currentMoveDirection = Define.MoveDirection.None;

    // 아래점프시 바닥 충돌 무시할 시간
    protected WaitForSeconds ignorePlatTime_wait;

    // 공격 관련 변수
    public Transform atkPos;
    public Vector2 atkBoxSize;


    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioJump);
        TryGetComponent(out Stat);
        ignorePlatTime_wait = new WaitForSeconds(1f);
    }

    // 공격
    protected override void Attack()
    {
        // 기본 공격
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StopCoroutine(nameof(Attack_co));
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        bool isIdleOrWalking = (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni));
        bool leftArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightArrowPressed = Input.GetKey(KeyCode.RightArrow);

        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능
        if (isIdleOrWalking && !isImmobile)
        {
            if (leftArrowPressed && !rightArrowPressed)
            {
                movement.MoveTo(Vector2.left);
            }
            else if (!leftArrowPressed && rightArrowPressed)
            {
                movement.MoveTo(Vector2.right);
            }
            else
            {
                movement.MoveTo(Vector2.zero);
            }
            if (Input.GetButtonDown("Jump"))
            {
                movement.JumpTo();
            }
        }
        else
        {
            movement.MoveTo(new Vector2(rigid.velocity.x / Stat.Speed, 0));
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
                SoundManager.instance.PlaySfx(Define.Sfx.Jump);
                //GameManager.Sound.PlaySfx(Define.Sfx.Jump);
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

    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);


            // 자식 오브젝트 포함 모두 무적상태로 변경
            gameObject.layer = invincibleLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = invincibleLayer;
            }
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);


            StopCoroutine(nameof(OffDamaged_co));
            StartCoroutine(nameof(OffDamaged_co));
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
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


    IEnumerator Attack_co()
    {
        // 공격 가능한 상태일 때만 공격 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            SoundManager.instance.PlaySfx(Define.Sfx.AttackS);
            //GameManager.Sound.PlaySfx(Define.Sfx.AttackS);

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(atkPos.position, atkBoxSize, 0, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in collider2Ds)
            {
                collider.TryGetComponent(out MonsterControl monster);
                monster.OnDamaged((Vector2)transform.position);

                collider.TryGetComponent(out MonsterStat monsterData);
                int damage = 1;
                if (Stat.Atk - monsterData.Def > 1)
                {
                    damage = Stat.Atk - monsterData.Def;
                }
                monsterData.Hp -= damage;
            }
            anim.SetBool("isAttack", true);

            yield return null;
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
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
    }

    protected override IEnumerator OffDamaged_co()
    {
        // 무적시간 경과 후 원래 상태로 돌아옴
        yield return invincibleTime_wait;
        isImmobile = false;
        gameObject.layer = myLayer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = myLayer;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(atkPos.position, atkBoxSize);
    }
}
