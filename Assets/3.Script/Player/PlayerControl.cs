using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CreatureControl
{
    protected AudioClip audioJump;
    private Player Stat;

    public Define.MoveDirection currentMoveDirection = Define.MoveDirection.None;

    // �Ʒ������� �ٴ� �浹 ������ �ð�
    protected WaitUntil inputZ_wait;
    protected WaitUntil itemRootEnd_wait;
    protected WaitForSeconds itemRootDelay_wait;
    protected WaitForSeconds ignorePlatTime_wait;
    protected WaitForSeconds offHit_wait;
    protected WaitForSeconds recover_wait;
    private float offHitTime = 5f;
    private float recoverTime = 2f;

    // ���� ���� ����
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
        itemRootDelay_wait = new WaitForSeconds(0.1f);
        offHit_wait = new WaitForSeconds(offHitTime - invincibleTime);
        recover_wait = new WaitForSeconds(recoverTime);
        anim.SetBool("isNomal", true);

        StartCoroutine(nameof(RootItem_co));
    }

    // ����
    protected override void Attack()
    {
        //�⺻ ����
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StopCoroutine(nameof(Attack_co));
            StartCoroutine(nameof(Attack_co));
        }
    }

    protected override void Move()
    {
        bool isIdleOrWalking = (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
        bool leftArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightArrowPressed = Input.GetKey(KeyCode.RightArrow);

        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����
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
            // ���帮���� Ȯ��
            Prone();
            if (Input.GetButtonDown("Jump"))
            {
                SoundManager.instance.PlaySfx(Define.Sfx.Jump);
                //GameManager.Sound.PlaySfx(Define.Sfx.Jump);
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

    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            anim.SetBool("isNomal", false);
            // �ڽ� ������Ʈ ���� ��� �������·� ����
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


    private IEnumerator Attack_co()
    {
        // ���� ������ ������ ���� ���� ����
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni) ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            SoundManager.instance.PlaySfx(Define.Sfx.AttackS);
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
        // ���� �ð����� �浹 ������
        // ���� �� �ٴ��� ��� Platform Effector�� UseCollider Mask üũ�Ͽ� �Ʒ����� ����
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Physics2D.IgnoreLayerCollision(myLayer, groundLayer, false);
    }
    private IEnumerator RootItem_co()
    {
        while (true)
        {
            // ���� ��Ÿ��
            yield return itemRootDelay_wait;
            // z������ ������ ����
            yield return inputZ_wait;

            // Item ���̾ ����
            int layerMask = LayerMask.GetMask("Item");
            Collider2D col = Physics2D.OverlapCircle(transform.position, 1.0f, layerMask);

            // Item�� ���������� ������ ����
            if (col != null)
            {
                SoundManager.instance.PlaySfx(Define.Sfx.PickUpItem);
                // �̹� ������������ ���� �̵����� �������� �ٽ� ȹ������ ���ϵ��� �����ϱ� ���� ���̾� ����
                col.gameObject.layer = LayerMask.NameToLayer("ItemRoot");
                // ���� �ڷ�ƾ�� �� ������ �������� �������� �־ �ٸ� �����۵� ������ �� �ֵ��� �ϱ� ���ؼ�
                StartCoroutine(MoveItem_co(col));
            }
        }
    }
    private IEnumerator MoveItem_co(Collider2D col)
    {
        // ������ �̵� �ӵ�
        float moveSpeed = 2.0f;
        // �������� �÷��̾�� ������������� �̵�
        while (Mathf.Abs(col.gameObject.transform.position.x - transform.position.x) > 0.3f)
        {
            col.gameObject.transform.position = Vector2.Lerp(col.gameObject.transform.position, transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        // ���̸� ������
        if (col.gameObject.GetComponentInChildren<SpriteRenderer>().sprite.name[0].Equals('9'))
        {
            Stat.Gold += col.gameObject.GetComponent<FieldItem>().money;
        }
        // �� �ƴϸ� ������ ȹ��
        else
        {
            Stat.inventory.GetItem(int.Parse(col.gameObject.GetComponentInChildren<SpriteRenderer>().sprite.name));
        }
        Destroy(col.gameObject);
    }

    protected override IEnumerator OffDamaged_co()
    {
        // �����ð� ��� �� ���� ���·� ���ƿ�
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
