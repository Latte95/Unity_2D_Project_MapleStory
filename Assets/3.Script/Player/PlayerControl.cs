using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private Player Stat;
    private GameObject DamagePrefab;
    private GameObject SkillHitPrefab;
    Animator attackEffectAnimator;

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
    private Vector2 atkBoxSize;
    private Vector2 magicBoxSize;

    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioJump);
        TryGetComponent(out Stat);
        attackEffectAnimator = transform.Find("AttackEffect").gameObject.GetComponent<Animator>();
        ignorePlatTime_wait = new WaitForSeconds(0.2f);

        dieHp_wait = new WaitUntil(() => Stat.Hp <= 0);
        inputZ_wait = new WaitUntil(() => Input.GetKey(KeyCode.Z));
        inputUpOrDownArrow_wait = new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow));
        itemRootDelay_wait = new WaitForSeconds(0.1f);
        offHit_wait = new WaitForSeconds(offHitTime - invincibleTime);
        recover_wait = new WaitForSeconds(recoverTime);
        anim.SetBool("isNomal", true);

        StartCoroutine(nameof(RootItem_co));
        StartCoroutine(nameof(InputUpOrDownArrow_co));
        dieHp_wait = new WaitUntil(() => Stat.Hp <= 0);
        StartCoroutine(nameof(OnDie_co));
        DamagePrefab = Resources.Load<GameObject>("Damage/" + "Damage");
        SkillHitPrefab = Resources.Load<GameObject>("Effect/" + "SkillEffect");

        atkBoxSize = new Vector2(1, 1);
        magicBoxSize = new Vector2(4, 1);
    }

    protected override void Update()
    {
        IsOnGround();
        base.Update();

        // 땅에서 할 행동들. 공통 : 점프 / 플레이어 : 엎드리기
        GroundAct();
    }


    // 공격
    public override void Attack()
    {
        // 기본 공격
        StopCoroutine(nameof(Attack_co));
        StartCoroutine(nameof(Attack_co));
    }
    public void Magic()
    {
        // 매직클로
        StopCoroutine(nameof(Magic_co));
        StartCoroutine(nameof(Magic_co));
    }

    protected override void Move()
    {
        bool isIdleOrWalking = (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
        bool isRopeOrLadder = (anim.GetBool("isRope") || anim.GetBool("isLadder"));
        bool isJump = anim.GetCurrentAnimatorStateInfo(0).IsName(jumpAni);
        bool leftArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightArrowPressed = Input.GetKey(KeyCode.RightArrow);
        bool upPressed = Input.GetKey(KeyCode.UpArrow);
        bool downArrowPressed = Input.GetKey(KeyCode.DownArrow);
        Vector2 dir = Vector2.zero;
        Vector2 dirVer = Vector2.zero;

        int flip = transform.localScale.x > 0 ? 1 : -1;
        Stat.levelUpEffect.transform.localScale = new Vector3(flip, 1, 1);

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
                rigid.AddForce(10 * Vector2.up, ForceMode2D.Impulse);
                RopeOff();
                GameManager.Instance.soundManager.PlaySfx(Define.Sfx.Jump);
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

    protected void GroundAct()
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
                GameManager.Instance.soundManager.PlaySfx(Define.Sfx.Jump);
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
            GameManager.Instance.soundManager.PlaySfx(Define.Sfx.AttackS);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
            {
                attackEffectAnimator.SetTrigger(Define.Skill.Attack.ToString());
            }

            Collider2D collider = Physics2D.OverlapBox(atkPos.position, atkBoxSize, 0, LayerMask.GetMask("Enemy"));
            if (collider != null)
            {
                collider.TryGetComponent(out MonsterControl monster);
                collider.TryGetComponent(out MonsterStat monsterData);
                if (monsterData.Hp <= 0)
                {
                    yield break;
                }

                monster.OnDamaged(transform.position);
                int damage = (Stat.AD) - monsterData.Def;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
                {
                    damage = (int)(damage * 0.1f);
                }
                if (damage < 1)
                {
                    damage = 1;
                }

                // 데미지 표시
                Vector3 position = collider.transform.position;
                StartCoroutine(DamageEffect_co(damage, position));

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
    private IEnumerator Magic_co()
    {
        // 공격 가능한 상태일 때만 공격 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            // mp부족하면 종료
            if (Stat.Mp < 50)
            {
                yield break;
            }
            else
            {
                Stat.Mp -= 50;
            }

            GameManager.Instance.soundManager.PlaySfx(Define.Sfx.Magic);
            attackEffectAnimator.SetTrigger(Define.Skill.MagicClaw.ToString());

            anim.SetBool("isMagic", true);
            yield return null;
            anim.SetBool("isMagic", false);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + transform.localScale.x * 2 * Vector3.left, magicBoxSize, 0, LayerMask.GetMask("Enemy"));
            int length = colliders.Length;
            if (length > 0)
            {
                // 가장 가까운 적 체크
                Collider2D closestEnemyCollider = colliders[0];
                float minDistance = Vector2.Distance(transform.position, colliders[0].transform.position);
                for (int i = 1; i < length; i++)
                {
                    float currentDistance = Vector2.Distance(transform.position, colliders[i].transform.position);
                    if (currentDistance < minDistance)
                    {
                        closestEnemyCollider = colliders[i];
                        minDistance = currentDistance;
                    }
                }

                // 가장 가까운 적에게 공격
                closestEnemyCollider.TryGetComponent(out MonsterControl monster);
                closestEnemyCollider.TryGetComponent(out MonsterStat monsterData);
                if (monsterData.Hp <= 0)
                {
                    yield break;
                }

                monster.OnDamaged(transform.position);

                GameObject SkillHit = Instantiate(SkillHitPrefab);
                SkillHit.transform.position = closestEnemyCollider.transform.position;
                SkillHit.GetComponent<Animator>().SetTrigger(Define.Skill.MagicClaw.ToString());

                int damageSum = 0;
                for (int i = 0; i < 2; i++)
                {
                    int damage = (Stat.AP) - monsterData.Def;
                    if (damage < 1)
                    {
                        damage = 1;
                    }
                    damageSum += damage;

                    Vector3 position = closestEnemyCollider.transform.position + (i + 1 - 0.5f) * new Vector3(0.7f, 0.3f);
                    StartCoroutine(DamageEffect_co(damage, position));
                }

                yield return new WaitForSeconds(0.1f);
                monster.HitSound();
                monsterData.Hp -= damageSum;
                if (monsterData.Hp <= 0)
                {
                    Stat.Exp += monsterData.Exp;
                }
            }
        }
    }
    private IEnumerator DamageEffect_co(int damage, Vector3 position)
    {
        // 데미지 화면에 띄우기
        GameObject Damage = Instantiate(DamagePrefab);
        Damage.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        Text damageText = Damage.GetComponent<Text>();

        // 수치 설정
        damageText.text = damage.ToString();
        // 위치 설정
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position + Vector3.up);
        Damage.transform.position = screenPoint;

        // 데미지 사라짐
        for (float alpha = 1f; alpha >= 0f; alpha -= 1.5f * Time.deltaTime)
        {
            Color newColor = damageText.color;
            newColor.a = alpha;
            damageText.color = newColor;
            yield return null;
            Damage.transform.position += 0.5f * Vector3.up;
        }
        Destroy(Damage);
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
                GameManager.Instance.soundManager.PlaySfx(Define.Sfx.PickUpItem);
                // 이미 루팅중이지만 아직 이동중인 아이템을 다시 획득하지 못하도록 방지하기 위한 레이어 변경
                col.gameObject.layer = LayerMask.NameToLayer("ItemRoot");
                // 이중 코루틴을 쓴 이유는 루팅중인 아이템이 있어도 다른 아이템도 루팅할 수 있도록 하기 위해서
                StartCoroutine(MoveItem_co(col));
            }
        }
    }


    public event Action OnMoneyAdded;
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
            OnMoneyAdded?.Invoke();
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
        while (true)
        {
            yield return dieHp_wait;
            yield return null;
            anim.SetTrigger("isDie");
            rigid.velocity = Vector2.zero;
            yield return recover_wait;
            Recovery();
        }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.localScale.x * 2 * Vector3.left, magicBoxSize);
    }
}
