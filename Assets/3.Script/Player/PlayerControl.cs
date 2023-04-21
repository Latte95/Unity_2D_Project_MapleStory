using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private Player Stat;

    public Define.MoveDirection currentMoveDirection = Define.MoveDirection.None;

    // 아래점프시 바닥 충돌 무시할 시간
    protected WaitUntil inputZ_wait;
    protected WaitUntil inputUpOrDownArrow_wait;
    protected WaitUntil itemRootEnd_wait;
    protected WaitForSeconds itemRootDelay_wait;
    protected WaitForSeconds ignorePlatTime_wait;
    protected WaitForSeconds offHit_wait;
    protected WaitForSeconds recover_wait;
    private float offHitTime = 5f;
    private float recoverTime = 2f;

    // 공격 관련 변수
    public Transform atkPos;
    public Vector2 atkBoxSize;

    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioJump);
        TryGetComponent(out Stat);
        ignorePlatTime_wait = new WaitForSeconds(0.2f);

        dieHp = new WaitUntil(() => Stat.Hp <= 0);
        inputZ_wait = new WaitUntil(() => Input.GetKey(KeyCode.Z));
        inputUpOrDownArrow_wait = new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow));
        itemRootDelay_wait = new WaitForSeconds(0.1f);
        offHit_wait = new WaitForSeconds(offHitTime - invincibleTime);
        recover_wait = new WaitForSeconds(recoverTime);
        anim.SetBool("isNomal", true);

        StartCoroutine(nameof(RootItem_co));
        StartCoroutine(nameof(InputUpOrDownArrow_co));
    }

    protected override void Update()
    {
        IsOnGround();
        base.Update();
    }


    // 공격
    protected override void Attack()
    {
        //기본 공격
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StopCoroutine(nameof(Attack_co));
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        bool isIdleOrWalking = (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
        bool isRopeOrLadder = (anim.GetBool("isRope") || anim.GetBool("isLadder"));
        bool leftArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightArrowPressed = Input.GetKey(KeyCode.RightArrow);
        bool upPressed = Input.GetKey(KeyCode.UpArrow);
        bool downArrowPressed = Input.GetKey(KeyCode.DownArrow);
        Vector2 dir = Vector2.zero;
        Vector2 dirVer = Vector2.zero;

        if (leftArrowPressed && !rightArrowPressed)
        {
            dir = Vector2.left;
        }
        else if (!leftArrowPressed && rightArrowPressed)
        {
            dir = Vector2.right;
        }
        if (upPressed && !downArrowPressed)
        {
            dirVer = Vector2.up;
        }
        else if (!upPressed && downArrowPressed)
        {
            dirVer = Vector2.down;
        }

        if (isRopeOrLadder)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rigid.velocity = dir;
                rigid.AddForce(10*Vector2.up,ForceMode2D.Impulse);
                RopeOff();
                SoundManager.Instance.PlaySfx(Define.Sfx.Jump);
            }
            else
            {
                rigid.velocity = (Vector3)dirVer;
            }
            if (dirVer.Equals(Vector2.zero))
            {
                anim.SetBool("isRopeMove", false);
            }
            else
            {
                anim.SetBool("isRopeMove", true);
            }
        }

        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능
        if (isIdleOrWalking && !isImmobile)
        {
            movement.MoveTo(dir);
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
            bool isRopeOrLadder = (anim.GetBool("isRope") || anim.GetBool("isLadder"));
            if (isRopeOrLadder)
            {
                RopeOff();
            }
            // 엎드리는지 확인
            Prone();
            if (Input.GetButtonDown("Jump") && !isRopeOrLadder)
            {
                SoundManager.Instance.PlaySfx(Define.Sfx.Jump);
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

    // 땅 밟은지 체크
    protected void IsOnGround()
    {
        // 발밑에서 박스캐스트 생성
        // ground나 slope만 체크
        RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down,
                                                    boxCastMaxDistance, LayerMask.GetMask(platLayer));
        // 바닥과 충돌
        if (raycastHit.collider != null)
        {
            bool isRopeOrLadder = (anim.GetBool("isRope") || anim.GetBool("isLadder"));

            if (rigid.velocity.y < 0.01f || isRopeOrLadder)
            {
                anim.SetBool("isGrounded", true);
                isGrounded = true;
                isImmobile = false;
                if (!raycastHit.collider.tag.Equals(lastGroundTag))
                {
                    Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(raycastHit.collider.gameObject.tag), false);

                }
                lastGroundTag = raycastHit.collider.gameObject.tag;
                if (rigid.velocity.y >= 0.01f)
                {
                    rigid.position += 0.4f * Vector2.up;
                }
            }

            // 경사면 체크
            if (raycastHit.collider.gameObject.layer.Equals(slopeLayer))
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
        }
        // 공중이면
        else
        {
            anim.SetBool("isGrounded", false);
            isGrounded = false;
            isSlope = false;
        }
    }
    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            anim.SetBool("isNomal", false);
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

    private IEnumerator Attack_co()
    {
        // 공격 가능한 상태일 때만 공격 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            SoundManager.Instance.PlaySfx(Define.Sfx.AttackS);
            //GameManager.Sound.PlaySfx(Define.Sfx.AttackS);

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(atkPos.position, atkBoxSize, 0, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in collider2Ds)
            {
                collider.TryGetComponent(out MonsterControl monster);
                monster.OnDamaged(transform.position);

                collider.TryGetComponent(out MonsterStat monsterData);
                int damage = Stat.Atk - monsterData.Def;
                if (damage < 1)
                {
                    damage = 1;
                }
                monsterData.Hp -= damage;
                if (monsterData.Hp <= 0)
                {
                    Stat.Exp += monsterData.Exp;
                }
            }
            anim.SetBool("isAttack", true);

            yield return null;
            anim.SetBool("isAttack", false);
        }
    }
    private IEnumerator DownJump_co()
    {
        // 일정 시간동안 충돌 무시함
        // 가장 밑 바닥일 경우 Platform Effector의 UseCollider Mask 체크하여 아래점프 방지
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
    }
    private IEnumerator RootItem_co()
    {
        while (true)
        {
            // 루팅 쿨타임
            yield return itemRootDelay_wait;
            // z눌르고 있으면 실행
            yield return inputZ_wait;

            // Item 레이어만 감지
            int layerMask = LayerMask.GetMask("Item");
            Collider2D col = Physics2D.OverlapCircle(transform.position, 1.0f, layerMask);

            // Item를 감지했으면 아이템 루팅
            if (col != null)
            {
                SoundManager.Instance.PlaySfx(Define.Sfx.PickUpItem);
                // 이미 루팅중이지만 아직 이동중인 아이템을 다시 획득하지 못하도록 방지하기 위한 레이어 변경
                col.gameObject.layer = LayerMask.NameToLayer("ItemRoot");
                // 이중 코루틴을 쓴 이유는 루팅중인 아이템이 있어도 다른 아이템도 루팅할 수 있도록 하기 위해서
                StartCoroutine(MoveItem_co(col));
            }
        }
    }
    private IEnumerator MoveItem_co(Collider2D col)
    {
        // 아이템 이동 속도
        float moveSpeed = 3.5f;
        // 아이템이 플레이어와 가까워질때까지 이동
        while (Mathf.Abs(col.gameObject.transform.position.x - transform.position.x) > 0.3f)
        {
            Transform trans = col.gameObject.transform;
            trans.position = Vector2.Lerp(trans.position, 2 * transform.position - trans.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        // 돈이면 돈증가
        if (col.gameObject.GetComponentInChildren<SpriteRenderer>().sprite.name[0].Equals('9'))
        {
            Stat.Gold += col.gameObject.GetComponent<FieldItem>().money;
        }
        // 돈 아니면 아이템 획득
        else
        {
            Stat.inventory.GetItem(int.Parse(col.gameObject.GetComponentInChildren<SpriteRenderer>().sprite.name));
        }
        Destroy(col.gameObject);
    }
    private IEnumerator InputUpOrDownArrow_co()
    {
        while (true)
        {
            yield return inputUpOrDownArrow_wait;
            if (anim.GetBool("isRope") || anim.GetBool("isLadder"))
            {
                continue;
            }
            bool upPressed = Input.GetKey(KeyCode.UpArrow);
            Vector3 dirVer = upPressed ? 0.5f * Vector3.up : Vector3.down;
            int layerMask = LayerMask.GetMask("Rope", "Ladder");
            Collider2D col = Physics2D.OverlapBox(transform.position + 0.7f * dirVer, 0.25f * Vector2.one, 0f, layerMask);

            if (col != null)
            {
                if (upPressed)
                {
                    transform.position += 0.1f * Vector3.up;
                }
                else
                {
                    transform.position -= 0.3f * Vector3.up;
                }
                switch (LayerMask.LayerToName(col.gameObject.layer))
                {
                    case "Rope":
                        anim.SetBool("isRope", true);
                        transform.position = new Vector2(col.gameObject.transform.position.x - 0.24f, transform.position.y);
                        break;
                    case "Ladder":
                        anim.SetBool("isLadder", true);
                        transform.position = new Vector2(col.gameObject.transform.position.x, transform.position.y);
                        break;
                }
                rigid.gravityScale = 0;
            }
        }
    }
    private void RopeOff()
    {
        anim.SetBool("isRope", false);
        anim.SetBool("isLadder", false);
        anim.SetBool("isRopeMove", false);
        rigid.gravityScale = 1;
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
        yield return offHit_wait;
        anim.SetBool("isNomal", true);
    }
    protected override IEnumerator OnDie_co()
    {
        yield return dieHp;
        yield return null;
        anim.SetTrigger("isDie");
        rigid.velocity = Vector2.zero;
        yield return recover_wait;
        Recovery();
    }

    private void Recovery()
    {
        anim.Play("Idle");
        Stat.Hp = Stat.MaxHp;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(atkPos.position, atkBoxSize);
    }
}
