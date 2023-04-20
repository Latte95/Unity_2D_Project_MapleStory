using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : CreatureControl
{
    [SerializeField]
    protected AudioClip[] sfxClips;
    AudioSource sfxPlayer;

    private MonsterStat Stat;
    int dir = 0;


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

        dieHp = new WaitUntil(() => Stat.Hp <= 0);
        StartCoroutine(nameof(Direct_co));
        // 점프
        StartCoroutine(nameof(Jump_co));

        // OnEnable에 있으면 시작할 때 죽음
        StartCoroutine(nameof(OnDie_co));
    }
    private new void Start()
    {
        base.Start();
        // 이동 방향 결정
    }

    // 지금은 몬스터 공격이 없지만, 공격패턴을 갖는 몬스터를 만들때를 대비
    protected override void Attack() { }

    protected override void Move()
    {
        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능
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
        itemInstance.transform.position = transform.position+0.2f*Vector3.left;
        int rand = Random.Range(1, 10);
        if(rand < 8)
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
        if(moneyIndex < 50)
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
}
