using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾�� ���� ���� �Ӽ�
public abstract class CreatureControl : MonoBehaviour
{
    #region ������Ʈ
    protected Movement movement;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected AudioSource audioSource;
    protected Transform FootTrans;
    #endregion ������Ʈ

    #region �ٴ� Ȯ��
    [SerializeField]
    // �ٴ�, ���� Ȯ���ϴ� �ڽ�ĳ��Ʈ
    protected Vector2 boxCastSize;
    protected float boxCastMaxDistance = 0.0001f;

    // �� ��� �ִ���
    protected bool isGrounded = false;
    // ���� ��� �ִ���
    protected bool isSlope = false;
    
    // ���� �ֱٿ� ���� ���� �±��̸� -> �̸� �̿��ؼ� �� �հ� ������
    protected string lastGroundTag;
    #endregion �ٴ�


    // �ǰݽ� ���� �ð����� �̵� �Ұ�, �ٴ���Ʈ ������ ����
    public bool isImmobile = false;

    #region ĳ��
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
    protected WaitUntil dieAni_wait;
    protected WaitUntil dieHp_wait;
    #endregion ĳ��

    #region �ϵ��ڵ� ����
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
    #endregion �ϵ��ڵ�

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
        // ��Ż�� ��� ���� �� �̲����� ����
        if (isSlope && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // ����� ���� ����
        CheckWall();
        Move();
    }


    // �ǰݴ���
    public virtual void OnDamaged(Vector2 targetPos)
    {
        // �̵� �Ұ���, �ٴ���Ʈ ����
        isImmobile = true;

        // �ǰݿ� ���� �и�
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);

        // �ǰݻ��� ����
        StopCoroutine(nameof(OffDamaged_co));
        StartCoroutine(nameof(OffDamaged_co));
    }

    private void CheckWall()
    {
        int dir = 0;
        // �ٷ� �� �浹ü �����ϴ� ����ĳ��Ʈ
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

        // �±� �ٸ��� �� ��� o 
        if (raycastHit.collider != null && !raycastHit.collider.CompareTag(lastGroundTag))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, true);
        }
    }

    // �ڽ��� ����ִ� ���� �ٸ� ������ ���� ���
    IEnumerator CheckWall_co()
    {
        // �ٷ� �� �浹ü �����ϴ� ����ĳ��Ʈ
        int dir = transform.localScale.x < 0 ? 1 : -1;
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(wallLayer));

        // �±� �ٸ��� �� ��� o 
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

    // ���信�� ����ĳ��Ʈ �׸�
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // �ٴ� üũ
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
        // �� üũ
        Vector2 boxCastSize2 = new Vector2(0.3f, 1f);
        Vector3 boxCastOrigin = transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right;
        Gizmos.DrawWireCube(boxCastOrigin, boxCastSize2);
    }
}
