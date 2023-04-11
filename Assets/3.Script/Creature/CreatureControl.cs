using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlManager : MonoBehaviour
{
    [SerializeField]
    protected Movement movement;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;

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
    protected string lastGroundTag;

    // 캐싱
    //
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
    // 발 위치
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
    protected string moveAni = "Move";
    protected string[] platLayer = new string[] { "Ground", "Slope" };
    [SerializeField]
    // 무적 시간
    protected float invincibleTime = 2f;


    protected void Awake()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);

        invincibleLayer = LayerMask.NameToLayer("Invincible");
        groundLayer = LayerMask.NameToLayer("Ground");
        slopeLayer = LayerMask.NameToLayer("Slope");
        invincibleTime_wait = new WaitForSeconds(invincibleTime);
        deltaTime = Time.deltaTime;
    }

    protected void Start()
    {
        myLayer = gameObject.layer;
        FootTrans = gameObject.transform.GetChild(1);
    }

    protected void Update()
    {
        // 땅 밟고 있는지 체크
        IsOnGround();

        // 비탈길 밟고 있을 시 미끄러짐 방지
        if (isSlope)
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
        // 바닥과 충돌중이고, 올라가는중이 아닐때만 체크
        // 0.1을 한 이유는 비탈길에서 가만히 있어도 조금씩 올라가기 때문
        if (raycastHit.collider != null && rigid.velocity.y <= 0.1)
        {
            isImmobile = false;
            isGrounded = true;
            anim.SetBool("isGrounded", true);
            if (raycastHit.collider.gameObject.layer.Equals(slopeLayer))
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
            lastGroundTag = raycastHit.collider.tag;
        }
        // 땅을 밟고 있지 않으면
        else
        {
            anim.SetBool("isGrounded", false);
            isSlope = false;
            isGrounded = false;
        }
        //return (raycastHit.collider != null);
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
        int dir = spriteRenderer.flipX ? 1 : -1;
        // 벽 레이어 이름
        string[] platLayer = new string[] { "Middle", "Bottom" };
        // 캐릭터 바로앞의 충돌을 감지할 박스캐스트
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + dir * 0.25f * Vector3.right, new Vector2(1f, 1f), 0f, dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(platLayer));

        // 가장 최근 밟았던 바닥의 태그 이름과 캐릭터 앞의 벽 레이어 이름이 일치하면
        // 벽과 충돌해야함
        if (raycastHit.collider != null && raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, false);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, false);
        }
        // 태그와 벽 레이어 이름이 일치하지 않으면
        // 벽과 충돌하면 안됨
        else if (raycastHit.collider != null && !raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, true);
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
            int dir = spriteRenderer.flipX ? 1 : -1;
            Vector2 boxCastSize2 = new Vector2(1f, 1f);
            Vector3 boxCastOrigin = transform.position + dir * 0.25f * Vector3.right;
            Gizmos.DrawWireCube(boxCastOrigin, boxCastSize2);
        }

    }
}
