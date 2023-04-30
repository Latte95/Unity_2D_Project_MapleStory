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

    #region 드롭템
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
    #endregion 템

    private new void Awake()
    {
        base.Awake();
        TryGetComponent(out Stat);

        // 사운드 세팅
        TryGetComponent(out sfxPlayer);
        sfxPlayer.playOnAwake = false;

        fieldItemTrans = GameObject.FindGameObjectWithTag("FieldItems").transform;

        dieHp_wait = new WaitUntil(() => Stat.Hp <= 0);
    }

    // 리스폰마다 초기화 할 것들
    private new void OnEnable()
    {
        base.OnEnable();

        onHP = false;
        // 리스폰 될 때 Hp 초기화
        Stat.Init();

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        // 이동
        StartCoroutine(nameof(Direct_co));
        StartCoroutine(nameof(Jump_co)); DamagePrefab = Resources.Load<GameObject>("Damage/" + "Damage");

        // 사망 판단
        StartCoroutine(nameof(OnDie_co));
    }

    protected override void Update()
    {
        IsOnGround();
        base.Update();
    }

    private void FixedUpdate()
    {
        // 몬스터 앞 바닥 체크
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

    // 지금은 몬스터 공격이 없지만, 공격패턴을 갖는 몬스터를 만들때를 대비
    public override void Attack() { }

    protected override void Move()
    {
        bool isJump = anim.GetCurrentAnimatorStateInfo(0).IsName(jumpAni);
        bool isWalk = anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni);
        bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName(idleAni);

        // 바닥 끝에서 뒤돌기
        if (isEndPlat && dir * transform.localScale.x < 0 && !isJump && !lastGroundTag.Equals(platLayer[2]))
        {
            dir *= -1;
        }

        // 이동
        if ((isIdle || isWalk) && !isImmobile)
        {
            movement.MoveTo(dir * Vector2.right);
        }
        // 점프중 속도 유지
        else
        {
            movement.MoveTo(new Vector2(rigid.velocity.x / Stat.Speed, 0));
        }
    }

    // 데미지 입음
    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            // HP바 표시
            onHP = true;
            // 죽는 중이 아닐때만 피격당함
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
            if (rigid.velocity.y < 0.01f)
            {
                anim.SetBool("isGrounded", true);
                isGrounded = true;
                isImmobile = false;
                // 새로운 땅 밟았으면 해당 벽과 충돌
                if (!raycastHit.collider.tag.Equals(lastGroundTag))
                {
                    lastGroundTag = raycastHit.collider.gameObject.tag;
                    Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), false);
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어랑 부딪히면 플레이어에게 데미지 입힘
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                // 죽는 중에는 플레이어에게 데미지 x
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
        // 무적시간 경과 후 원래 상태로 돌아옴
        yield return invincibleTime_wait;
        isImmobile = false;
        onHP = false;
    }
    protected override IEnumerator OnDie_co()
    {
        yield return dieHp_wait;
        // 피격 애니메이션에 의해 죽는 애니메이션 무시되는 것 방지
        yield return null;
        gameObject.layer = invincibleLayer;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("isDie");
        sfxPlayer.PlayOneShot(sfxClips[1]);

        // 포션 드랍
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

        // 기타템 드랍
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


        // 메소 드랍
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



        // 죽는 애니메이션 끝나면 비활성화
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down);
    }
}
