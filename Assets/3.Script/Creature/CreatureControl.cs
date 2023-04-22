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
    // �ٴ�, ���� Ȯ���ϴ� �ڽ�ĳ��Ʈ
    protected Vector2 boxCastSize;
    // �ڽ�ĳ��Ʈ �浹 ���� �Ÿ�
    protected float boxCastMaxDistance = 0.0001f;

    [SerializeField]
    // �� ��� �ִ���
    protected bool isGrounded = false;
    // ���� ��� �ִ���
    protected bool isSlope = false;
    public bool IsSlope => isSlope;
    // �ǰݽ� ���� �ð����� �̵� �Ұ�, �ٴ���Ʈ ������ ����
    [SerializeField]
    protected bool isImmobile = false;
    // ���� �ֱٿ� ���� ���� �±��̸� -> �̸� �̿��ؼ� �� �հ� ������
    [SerializeField]
    protected string lastGroundTag;

    // ĳ��
    //
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
    protected WaitUntil dieAni;
    protected WaitUntil dieHp;
    // �� ��ġ
    [SerializeField]
    protected Transform FootTrans;

    // �ϵ��ڵ�
    //
    protected int myLayer;
    // ���� ���̾� ��ȣ
    protected int invincibleLayer;
    // �� ���̾� ��ȣ
    protected int groundLayer;
    protected int slopeLayer;
    // �̵� �ִϸ��̼� �̸�
    protected string walkAni = "Walk";
    protected string[] platLayer = new string[] { "Ground", "Slope", "Other" };
    protected string[] wallLayer = new string[] { "Bottom", "Front", "Middle" };
    [SerializeField]
    // ���� �ð�
    protected float invincibleTime = 2f;


    protected virtual void Awake()
    {
        invincibleLayer = LayerMask.NameToLayer("Invincible");
        groundLayer = LayerMask.NameToLayer("Ground");
        slopeLayer = LayerMask.NameToLayer("Slope");
        invincibleTime_wait = new WaitForSeconds(invincibleTime);
        lastGroundTag = "Bottom";
        deltaTime = Time.deltaTime;
    }

    protected virtual void OnEnable()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out audioSource);
        dieAni = new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f &&
                                              anim.GetCurrentAnimatorStateInfo(0).IsName("Die"));
    }

    protected virtual void Start()
    {
        myLayer = gameObject.layer;
        FootTrans = gameObject.transform.GetChild(1);
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
        StartCoroutine(nameof(CheckWall_co));
        Move();

        // ������ �� �ൿ��. ���� : ���� / �÷��̾� : ���帮��
        GroundAct();
    }

    public virtual void OnDamaged(Vector2 targetPos)
    {
        // �̵� �Ұ���, �ٴ���Ʈ ����
        isImmobile = true;
        // �ǰ� �и� ������ �ӵ��� 0���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        // �ǰݿ� ���� �и�
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;

        rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);
        StopCoroutine(nameof(OffDamaged_co));
        StartCoroutine(nameof(OffDamaged_co));
    }

    protected abstract IEnumerator OffDamaged_co();
    protected abstract IEnumerator OnDie_co();

    // �ڽ��� ����ִ� ���� �ٸ� ������ ���� ���
    IEnumerator CheckWall_co()
    {
        // ����ĳ��Ʈ ���� ����
        int dir = transform.localScale.x < 0 ? 1 : -1;

        // ĳ���� �ٷξ��� �浹�� ������ �ڽ�ĳ��Ʈ
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + 0.5f * (-transform.localScale.x * 0.6f) * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(wallLayer));

        // �±� �ٸ��� �� ��� o 
        if (raycastHit.collider != null && !raycastHit.collider.CompareTag(lastGroundTag))
        {
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }


    public abstract void Attack();
    protected abstract void Move();
    protected abstract void GroundAct();

    // ���信�� ����ĳ��Ʈ �׸�
    protected virtual void OnDrawGizmos()
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
