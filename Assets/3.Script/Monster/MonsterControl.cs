using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : CreatureControl
{
    [SerializeField]
    protected AudioClip[] sfxClips;
    AudioSource sfxPlayer;

    private MonsterStat Stat;
    public int dir = 0;

    [SerializeField]
    private bool isEndPlat = false;


    public GameObject itemPrefab;
    public int[] itemImage;
    public int[] moneyImage;
    public int minMoney;
    public int maxMoney;

    private new void OnEnable()
    {
        base.OnEnable();

        // ������ �� �� Hp �ʱ�ȭ
        TryGetComponent(out Stat);
        Stat.Init();

        // ���� ����
        TryGetComponent(out sfxPlayer);
        sfxPlayer.playOnAwake = false;

        StartCoroutine(nameof(Direct_co));
        // ����
        StartCoroutine(nameof(Jump_co));

        dieHp = new WaitUntil(() => Stat.Hp <= 0);
        StartCoroutine(nameof(OnDie_co));
    }
    private new void Start()
    {
        base.Start();
        // �̵� ���� ����
    }
    protected override void Update()
    {
        IsOnGround();
        base.Update();
    }
    private void FixedUpdate()
    {
        // ĳ���� �ٷξ��� �浹�� ������ �ڽ�ĳ��Ʈ
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down, 1f, LayerMask.GetMask(platLayer));
        // �ٴ� ������ ���� 
        if (raycast.collider != null)
        {
            isEndPlat = false;
        }
        else
        {
            isEndPlat = true;
        }
    }

    // ������ ���� ������ ������, ���������� ���� ���͸� ���鶧�� ���
    protected override void Attack() { }

    protected override void Move()
    {
        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����
        if (isEndPlat && dir * transform.localScale.x < 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !lastGroundTag.Equals("Other"))
        {
            dir *= -1;
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni)) &&
            !isImmobile)
        {
            movement.MoveTo(dir * Vector2.right);
        }
        // �������� �ƴҶ��� �̵� ����
        // => �����߿� x�� �ӵ� ��ȭ x
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(new Vector2(rigid.velocity.x / Stat.Speed, 0));
        }
    }

    protected override void GroundAct()
    {
    }

    // ������ ����
    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            // �״� ���� �ƴҶ��� �ǰݴ���
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                sfxPlayer.PlayOneShot(sfxClips[0]);
                anim.SetTrigger("isHit");
            }
        }
    }

    // �� ������ üũ
    protected void IsOnGround()
    {
        // �߹ؿ��� �ڽ�ĳ��Ʈ ����
        // ground�� slope�� üũ
        RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down,
                                                    boxCastMaxDistance, LayerMask.GetMask(platLayer));
        // �ٴڰ� �浹
        if (raycastHit.collider != null)
        {
            if (rigid.velocity.y < 0.01f)
            {
                anim.SetBool("isGrounded", true);
                isGrounded = true;
                isImmobile = false;
                if (!raycastHit.collider.tag.Equals(lastGroundTag))
                {
                    Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(raycastHit.collider.gameObject.tag), false);
                }
                lastGroundTag = raycastHit.collider.gameObject.tag;
            }

            // ���� üũ
            if (raycastHit.collider.gameObject.layer.Equals(slopeLayer))
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
        }
        // �����̸�
        else
        {
            anim.SetBool("isGrounded", false);
            isGrounded = false;
            isSlope = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �ε����� �÷��̾�� ������ ����
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                // �״� �߿��� �÷��̾�� ������ x
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
                {
                    collision.gameObject.TryGetComponent(out PlayerControl playerCon);
                    playerCon.OnDamaged(transform.position);
                    int damage = Stat.Atk - player.Def;
                    if (damage < 1)
                    {
                        damage = 1;
                    }
                    player.Hp -= damage;

                }
            }
        }
    }

    protected override IEnumerator OffDamaged_co()
    {
        // �����ð� ��� �� ���� ���·� ���ƿ�
        yield return invincibleTime_wait;
        isImmobile = false;
    }
    protected override IEnumerator OnDie_co()
    {
        yield return dieHp;
        // �ǰ� �ִϸ��̼ǿ� ���� �״� �ִϸ��̼� ���õǴ� �� ����
        yield return null;
        anim.SetTrigger("isDie");
        sfxPlayer.PlayOneShot(sfxClips[1]);

        // ������ ��� ����
        GameObject itemInstance = Instantiate(itemPrefab);
        itemInstance.transform.position = transform.position + 0.2f * Vector3.left;
        int rand = Random.Range(1, 10);
        if (rand < 8)
        {
            rand = 0;
        }
        else
        {
            rand = 1;
        }
        itemInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ItemIcon/" + itemImage[rand].ToString());

        GameObject moneyInstance = Instantiate(itemPrefab);
        moneyInstance.transform.position = transform.position + 0.2f * Vector3.right;
        moneyInstance.GetComponent<FieldItem>().money = Random.Range(minMoney, maxMoney + 1);
        int moneyIndex = moneyInstance.GetComponent<FieldItem>().money;
        if (moneyIndex < 50)
        {
            moneyIndex = 0;
        }
        else if (moneyIndex < 100)
        {
            moneyIndex = 1;
        }
        else if (moneyIndex < 1000)
        {
            moneyIndex = 2;
        }
        else
        {
            moneyIndex = 3;
        }
        moneyInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MoneyIcon/" + moneyImage[moneyIndex].ToString());



        // �״� �ִϸ��̼� ������ ��Ȱ��ȭ
        transform.position = -25 * Vector2.one;
        yield return dieAni;
        gameObject.SetActive(false);
    }
    private IEnumerator Direct_co()
    {
        while (true)
        {
            dir = Random.Range(-1, 2);
            int wait = Random.Range(30, 55);
            yield return new WaitForSeconds(wait * 0.1f);
        }
    }
    private IEnumerator Jump_co()
    {
        while (true)
        {
            int wait = Random.Range(50, 90);
            yield return new WaitForSeconds(wait * 0.1f);
            if (isGrounded && anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni))
            {
                movement.JumpTo();
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down);
    }
}
