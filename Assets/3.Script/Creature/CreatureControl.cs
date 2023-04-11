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
    // 피격시 일정 시간동안 이동 불가
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
        isGrounded = IsOnGround();

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
    protected bool IsOnGround()
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
            anim.SetBool("isGrounded", true);
            if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Slope"))
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
            lastGroundTag = raycastHit.collider.tag;
        }
        else
        {
            anim.SetBool("isDown", false);
            anim.SetBool("isGrounded", false);
            isSlope = false;
        }
        return (raycastHit.collider != null);
    }

    protected void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            rigid.velocity = Vector3.zero;
            isImmobile = true;
            gameObject.layer = invincibleLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = invincibleLayer;
            }
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            StartCoroutine(nameof(OffDamaged_co));

            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);
        }
    }


    IEnumerator OffDamaged_co()
    {
        yield return invincibleTime_wait;
        isImmobile = false;
        gameObject.layer = myLayer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = myLayer;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    IEnumerator CheckWall_co()
    {
        int dir = spriteRenderer.flipX ? 1 : -1;
        string[] str = new string[] { "Middle", "Bottom" };
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + dir * 0.5f * Vector3.right, new Vector2(0.2f, 1f), 0f, dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(str));
        if (raycastHit.collider != null && raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)) &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, false);
        }
        else if (raycastHit.collider != null && !raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }


    protected abstract void Attack();
    protected abstract void Move();
    protected abstract void GroundAct();

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
            Vector3 gizmoPosition = transform.position + new Vector3(dir * 0.15f, 0f, 0f);
            Gizmos.DrawWireCube(gizmoPosition, new Vector3(0.35f, 1f, 1f));
        }

    }
}
