using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어와 몬스터 공통 속성
public abstract class CreatureControl : MonoBehaviour
{
    #region 컴포넌트
    protected Movement movement;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected AudioSource audioSource;
    protected Transform FootTrans;
    #endregion 컴포넌트

    #region 바닥 확인
    [SerializeField]
    // 바닥, 경사면 확인하는 박스캐스트
    protected Vector2 boxCastSize;
    protected float boxCastMaxDistance = 0.0001f;

    // 땅 밟고 있는지
    protected bool isGrounded = false;
    // 경사면 밟고 있는지
    protected bool isSlope = false;
    
    // 가장 최근에 밟은 땅의 태그이름 -> 이를 이용해서 벽 뚫고 지나감
    protected string lastGroundTag;
    #endregion 바닥


    // 피격시 일정 시간동안 이동 불가, 다단히트 방지도 겸함
    public bool isImmobile = false;

    #region 캐싱
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
    protected WaitUntil dieAni_wait;
    protected WaitUntil dieHp_wait;
    #endregion 캐싱

    #region 하드코딩 방지
    [SerializeField]
    protected int myLayer;
    protected int invincibleLayer;
    protected int groundLayer;
    protected int slopeLayer;
    protected string[] platLayer = new string[] { "Ground", "Slope", "Other" };
    protected string[] wallLayer = new string[] { "Bottom", "Front", "Middle" };

    protected string walkAni = "Walk";
    protected string dieAni = "Die";
    protected string jumpAni = "Jump";
    protected string idleAni = "Idle";
    [SerializeField]
    protected float invincibleTime = 2f;
    #endregion 하드코딩

    protected virtual void Awake()
    {
        invincibleLayer = LayerMask.NameToLayer("Invincible");
        groundLayer = LayerMask.NameToLayer(platLayer[0]);
        slopeLayer = LayerMask.NameToLayer(platLayer[1]);
        invincibleTime_wait = new WaitForSeconds(invincibleTime);
        lastGroundTag = wallLayer[0];
        deltaTime = Time.deltaTime;
        myLayer = gameObject.layer;
        FootTrans = gameObject.transform.Find("Foot");
    }

    protected virtual void OnEnable()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out audioSource);
        dieAni_wait = new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f &&
                                              anim.GetCurrentAnimatorStateInfo(0).IsName(dieAni));
    }

    protected virtual void Update()
    {
        // 비탈길 밟고 있을 시 미끄러짐 방지
        if (isSlope && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 벽통과 여부 결정
        CheckWall();
        Move();
    }


    // 피격당함
    public virtual void OnDamaged(Vector2 targetPos)
    {
        // 이동 불가능, 다단히트 방지
        isImmobile = true;

        // 피격에 의한 밀림
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);

        // 피격상태 해제
        StopCoroutine(nameof(OffDamaged_co));
        StartCoroutine(nameof(OffDamaged_co));
    }

    private void CheckWall()
    {
        int dir = 0;
        // 바로 앞 충돌체 감지하는 레이캐스트
        if(transform.localScale.x<0)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(wallLayer));

        // 태그 다르면 벽 통과 o 
        if (raycastHit.collider != null && !raycastHit.collider.CompareTag(lastGroundTag))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, true);
        }
    }

    // 자신이 밟고있는 땅과 다른 계층의 벽은 통과
    IEnumerator CheckWall_co()
    {
        // 바로 앞 충돌체 감지하는 레이캐스트
        int dir = transform.localScale.x < 0 ? 1 : -1;
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(wallLayer));

        // 태그 다르면 벽 통과 o 
        if (raycastHit.collider != null && !raycastHit.collider.CompareTag(lastGroundTag))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }


    public abstract void Attack();
    protected abstract void Move();

    protected abstract IEnumerator OffDamaged_co();
    protected abstract IEnumerator OnDie_co();

    // 씬뷰에서 레이캐스트 그림
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // 바닥 체크
        if (FootTrans != null)
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground", "Slope"));

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
        }
        // 벽 체크
        Vector2 boxCastSize2 = new Vector2(0.3f, 1f);
        Vector3 boxCastOrigin = transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right;
        Gizmos.DrawWireCube(boxCastOrigin, boxCastSize2);
    }
}
