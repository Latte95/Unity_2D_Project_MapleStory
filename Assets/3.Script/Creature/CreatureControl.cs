using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureControl : MonoBehaviour
{
    [SerializeField]
    protected Movement movement;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected AudioSource audioSource;

    [SerializeField]
    // 바닥, 경사면 확인하는 박스캐스트
    protected Vector2 boxCastSize;
    // 박스캐스트 충돌 감지 거리
    protected float boxCastMaxDistance = 0.05f;

    [SerializeField]
    // 땅 밟고 있는지
    protected bool isGrounded = false;
    // 경사면 밟고 있는지
    protected bool isSlope = false;
    public bool IsSlope => isSlope;
    // 피격시 일정 시간동안 이동 불가, 다단히트 방지도 겸함
    protected bool isImmobile = false;
    // 가장 최근에 밟은 땅의 태그이름 -> 이를 이용해서 벽 뚫고 지나감
    [SerializeField]
    protected string lastGroundTag;

    // 캐싱
    //
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
    // 발 위치
    [SerializeField]
    protected Transform FootTrans;

    // 하드코딩
    //
    protected int myLayer;
    // 무적 레이어 번호
    protected int invincibleLayer;
    // 땅 레이어 번호
    protected int groundLayer;
    protected int slopeLayer;
    // 이동 애니메이션 이름
    protected string walkAni = "Walk";
    protected string[] platLayer = new string[] { "Ground", "Slope", "Other" };
    protected string[] wallLayer = new string[] { "Bottom", "Front", "Middle" };
    [SerializeField]
    // 무적 시간
    protected float invincibleTime = 2f;


    protected virtual void Awake()
    {
        invincibleLayer = LayerMask.NameToLayer("Invincible");
        groundLayer = LayerMask.NameToLayer("Ground");
        slopeLayer = LayerMask.NameToLayer("Slope");
        invincibleTime_wait = new WaitForSeconds(invincibleTime);
        deltaTime = Time.deltaTime;
    }

    protected virtual void OnEnable()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out audioSource);
    }

    protected virtual void Start()
    {
        myLayer = gameObject.layer;
        FootTrans = gameObject.transform.GetChild(1);
    }

    protected void Update()
    {
        // 땅 밟고 있는지 체크
        IsOnGround();

        // 비탈길 밟고 있을 시 미끄러짐 방지
        if (isSlope && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        Attack();

        // 벽통과 여부 결정
        StartCoroutine(nameof(CheckWall_co));
        Move();

        // 땅에서 할 행동들. 공통 : 점프 / 플레이어 : 엎드리기
        GroundAct();
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
            if (rigid.velocity.y < 0.1f)
            {
                anim.SetBool("isGrounded", true);
                isGrounded = true;
                if (rigid.velocity.y < 0.5f)
                {
                    lastGroundTag = raycastHit.collider.tag;
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

    protected void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            // 이동 불가능, 다단히트 방지
            isImmobile = true;
            // 피격 밀림 이전에 속도를 0으로 초기화
            rigid.velocity = Vector3.zero;
            // 자식 오브젝트 포함 모두 무적상태로 변경
            gameObject.layer = invincibleLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = invincibleLayer;
            }
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            // 피격에 의한 밀림
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);

            StopCoroutine(nameof(OffDamaged_co));
            StartCoroutine(nameof(OffDamaged_co));

        }
    }

    protected abstract void PlaySound(string action);

    IEnumerator OffDamaged_co()
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
    // 자신이 밟고있는 땅과 다른 계층의 벽은 통과
    IEnumerator CheckWall_co()
    {
        // 레이캐스트 방향 설정
        int dir = transform.localScale.x < 0 ? 1 : -1;

        // 캐릭터 바로앞의 충돌을 감지할 박스캐스트
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + 0.5f * (-transform.localScale.x * 0.75f) * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(wallLayer));

        // 밟은 땅이랑 벽이 같은 태그면 벽 통과 x
        if (raycastHit.collider != null && raycastHit.collider.CompareTag(lastGroundTag))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, false);
        }
        // 태그 다르면 벽 통과 o 
        else if (raycastHit.collider != null)
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }


    protected abstract void Attack();
    protected abstract void Move();
    protected abstract void GroundAct();

    // 씬뷰에서 레이캐스트 그림
    void OnDrawGizmos()
    {
        if (FootTrans != null)
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground", "Slope"));

            Gizmos.color = Color.red;
            if (raycastHit.collider != null)
            {
                Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
                Gizmos.DrawWireCube(FootTrans.position + Vector3.down * raycastHit.distance, boxCastSize);
            }
            else
            {
                Gizmos.DrawRay(transform.position, Vector2.down * boxCastMaxDistance);
                Gizmos.DrawWireCube(FootTrans.position + Vector3.down * raycastHit.distance, boxCastSize);
            }
            Vector2 boxCastSize2 = new Vector2(0.3f, 1f);
            Vector3 boxCastOrigin = transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right;
            Gizmos.DrawWireCube(boxCastOrigin, boxCastSize2);
        }
    }
}
