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

        // 리스폰 될 때 Hp 초기화
        TryGetComponent(out Stat);
        Stat.Init();

        // 사운드 세팅
        TryGetComponent(out sfxPlayer);
        sfxPlayer.playOnAwake = false;

        StartCoroutine(nameof(Direct_co));
        // 점프
        StartCoroutine(nameof(Jump_co));

        dieHp = new WaitUntil(() => Stat.Hp <= 0);
        StartCoroutine(nameof(OnDie_co));
    }
    private new void Start()
    {
        base.Start();
        // 이동 방향 결정
    }
    protected override void Update()
    {
        IsOnGround();
        base.Update();
    }
    private void FixedUpdate()
    {
        // 캐릭터 바로앞의 충돌을 감지할 박스캐스트
        RaycastHit2D raycast = Physics2D.Raycast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, Vector2.down, 1f, LayerMask.GetMask(platLayer));
        // 바닥 없으면 멈춤 
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
    protected override void Attack() { }

    protected override void Move()
    {
        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능
        if (isEndPlat && dir * transform.localScale.x < 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !lastGroundTag.Equals("Other"))
        {
            dir *= -1;
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni)) &&
            !isImmobile)
        {
            movement.MoveTo(dir * Vector2.right);
        }
        // 점프중이 아닐때만 이동 멈춤
        // => 점프중에 x축 속도 변화 x
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(new Vector2(rigid.velocity.x / Stat.Speed, 0));
        }
    }

    protected override void GroundAct()
    {
    }

    // 데미지 입음
    public override void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos);
            // 죽는 중이 아닐때만 피격당함
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                sfxPlayer.PlayOneShot(sfxClips[0]);
                anim.SetTrigger("isHit");
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
        // 무적시간 경과 후 원래 상태로 돌아옴
        yield return invincibleTime_wait;
        isImmobile = false;
    }
    protected override IEnumerator OnDie_co()
    {
        yield return dieHp;
        // 피격 애니메이션에 의해 죽는 애니메이션 무시되는 것 방지
        yield return null;
        anim.SetTrigger("isDie");
        sfxPlayer.PlayOneShot(sfxClips[1]);

        // 아이템 드랍 로직
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



        // 죽는 애니메이션 끝나면 비활성화
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
