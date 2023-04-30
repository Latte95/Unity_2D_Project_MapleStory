using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterControl : CreatureControl
{
    [SerializeField]
    protected AudioClip[] sfxClips;
    AudioSource sfxPlayer;
    private Transform fieldItemTrans;

    private MonsterStat Stat;
    private GameObject DamagePrefab;
    private int dir = 0;

    private bool isEndPlat = false;
    public bool onHP = false;

    #region �����
    [Header("#Item")]
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private int[] itemImage;
    [SerializeField]
    private int[] itemProbability;
    [SerializeField]
    private int[] moneyImage;
    [SerializeField]
    private int minMoney;
    [SerializeField]
    private int maxMoney;
    #endregion ��

    private new void Awake()
    {
        base.Awake();
        TryGetComponent(out Stat);

        // ���� ����
        TryGetComponent(out sfxPlayer);
        sfxPlayer.playOnAwake = false;

        fieldItemTrans = GameObject.FindGameObjectWithTag("FieldItems").transform;

        dieHp_wait = new WaitUntil(() => Stat.Hp <= 0);
    }

    // ���������� �ʱ�ȭ �� �͵�
    private new void OnEnable()
    {
        base.OnEnable();

        onHP = false;
        // ������ �� �� Hp �ʱ�ȭ
        Stat.Init();

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        // �̵�
        StartCoroutine(nameof(Direct_co));
        StartCoroutine(nameof(Jump_co)); DamagePrefab = Resources.Load<GameObject>("Damage/" + "Damage");

        // ��� �Ǵ�
        StartCoroutine(nameof(OnDie_co));
    }

    protected override void Update()
    {
        IsOnGround();
        base.Update();
    }

    private void FixedUpdate()
    {
        // ���� �� �ٴ� üũ
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down, 1f,
                                                LayerMask.GetMask(platLayer));

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
    public override void Attack() { }

    protected override void Move()
    {
        bool isJump = anim.GetCurrentAnimatorStateInfo(0).IsName(jumpAni);
        bool isWalk = anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni);
        bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName(idleAni);

        // �ٴ� ������ �ڵ���
        if (isEndPlat && dir * transform.localScale.x < 0 && !isJump && !lastGroundTag.Equals(platLayer[2]))
        {
            dir *= -1;
        }

        // �̵�
        if ((isIdle || isWalk) && !isImmobile)
        {
            movement.MoveTo(dir * Vector2.right);
        }
        // ������ �ӵ� ����
        else
        {
            movement.MoveTo(new Vector2(rigid.velocity.x / Stat.Speed, 0));
        }
    }

    // ������ ����
    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            // HP�� ǥ��
            onHP = true;
            // �״� ���� �ƴҶ��� �ǰݴ���
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(dieAni))
            {
                HitSound();
                anim.SetTrigger("isHit");
            }
        }
    }
    public void HitSound()
    {
        sfxPlayer.PlayOneShot(sfxClips[0]);
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
                // ���ο� �� ������� �ش� ���� �浹
                if (!raycastHit.collider.tag.Equals(lastGroundTag))
                {
                    lastGroundTag = raycastHit.collider.gameObject.tag;
                    Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
                }
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
                    if (!playerCon.isImmobile)
                    {
                        playerCon.OnDamaged(transform.position);
                        int damage = Stat.Atk - player.Def;
                        if (damage < 1)
                        {
                            damage = 1;
                        }
                        player.Hp -= damage;
                        Vector3 position = collision.transform.position;
                        StartCoroutine(DamageEffect_co(damage, position));
                    }
                }
            }
        }
    }

    protected override IEnumerator OffDamaged_co()
    {
        // �����ð� ��� �� ���� ���·� ���ƿ�
        yield return invincibleTime_wait;
        isImmobile = false;
        onHP = false;
    }
    protected override IEnumerator OnDie_co()
    {
        yield return dieHp_wait;
        // �ǰ� �ִϸ��̼ǿ� ���� �״� �ִϸ��̼� ���õǴ� �� ����
        yield return null;
        gameObject.layer = invincibleLayer;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("isDie");
        sfxPlayer.PlayOneShot(sfxClips[1]);

        // ���� ���
        GameObject itemInstance = Instantiate(itemPrefab);
        itemInstance.transform.SetParent(fieldItemTrans, false);
        itemInstance.transform.position = transform.position + 0.2f * Vector3.left;
        int rand = Random.Range(0, itemProbability[0] + itemProbability[1]);
        if (rand < itemProbability[0])
        {
            itemInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ItemIcon/" + itemImage[0].ToString());
        }
        else
        {
            itemInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ItemIcon/" + itemImage[1].ToString());
        }

        // ��Ÿ�� ���
        if (itemImage.Length > 2)
        {
            rand = Random.Range(0, 100);
            if (rand < itemProbability[2])
            {
                GameObject etcInstance = Instantiate(itemPrefab);
                etcInstance.transform.SetParent(fieldItemTrans, false);
                etcInstance.transform.position = transform.position + 0.6f * Vector3.left;
                etcInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ItemIcon/" + itemImage[2].ToString());
            }
        }


        // �޼� ���
        GameObject moneyInstance = Instantiate(itemPrefab);
        moneyInstance.transform.SetParent(fieldItemTrans, false);
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
        yield return dieAni_wait;
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
    private IEnumerator DamageEffect_co(int damage, Vector3 position)
    {
        // ������ ȭ�鿡 ����
        GameObject Damage = Instantiate(DamagePrefab);
        Damage.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        Text damageText = Damage.GetComponent<Text>();

        // ��ġ ����
        damageText.text = damage.ToString();
        // ��ġ ����
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position + Vector3.up);
        Damage.transform.position = screenPoint;

        // ������ �����
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down);
    }
}
