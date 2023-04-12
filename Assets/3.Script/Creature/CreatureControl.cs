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
    protected float boxCastMaxDistance = 0.05f;

    [SerializeField]
    // �� ��� �ִ���
    protected bool isGrounded = false;
    // ���� ��� �ִ���
    protected bool isSlope = false;
    public bool IsSlope => isSlope;
    // �ǰݽ� ���� �ð����� �̵� �Ұ�, �ٴ���Ʈ ������ ����
    protected bool isImmobile = false;
    // ���� �ֱٿ� ���� ���� �±��̸� -> �̸� �̿��ؼ� �� �հ� ������
    [SerializeField]
    protected string lastGroundTag;

    // ĳ��
    //
    float deltaTime;
    protected WaitForSeconds invincibleTime_wait;
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
    protected string moveAni = "Move";
    protected string[] platLayer = new string[] { "Ground", "Slope", "Other" };
    [SerializeField]
    // ���� �ð�
    protected float invincibleTime = 2f;


    protected void Awake()
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
        // �� ��� �ִ��� üũ
        IsOnGround();

        // ��Ż�� ��� ���� �� �̲����� ����
        if (isSlope)
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        Attack();

        // ����� ���� ����
        StartCoroutine(nameof(CheckWall_co));
        Move();

        // ������ �� �ൿ��. ���� : ���� / �÷��̾� : ���帮��
        GroundAct();
    }

    // �� ������ üũ
    protected void IsOnGround()
    {
        // �߹ؿ��� �ڽ�ĳ��Ʈ ����
        // ground�� slope�� üũ
        RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down,
                                                    boxCastMaxDistance, LayerMask.GetMask(platLayer));
        // �ٴڰ� �浹���̰�, �ö󰡴����� �ƴҶ��� üũ
        // 0.1�� �� ������ ��Ż�濡�� ������ �־ ���ݾ� �ö󰡱� ����
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
        // ���� ��� ���� ������
        else
        {
            anim.SetBool("isGrounded", false);
            isSlope = false;
            isGrounded = false;
            Physics2D.IgnoreLayerCollision(myLayer, LayerMask.NameToLayer(lastGroundTag), true);
        }
        //return (raycastHit.collider != null);
    }

    protected void OnDamaged(Vector2 targetPos)
    {
        if (!isImmobile)
        {
            // �̵� �Ұ���, �ٴ���Ʈ ����
            isImmobile = true;
            // �ǰ� �и� ������ �ӵ��� 0���� �ʱ�ȭ
            rigid.velocity = Vector3.zero;
            // �ڽ� ������Ʈ ���� ��� �������·� ����
            gameObject.layer = invincibleLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = invincibleLayer;
            }
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            // �ǰݿ� ���� �и�
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(7 * new Vector2(dirc, 2), ForceMode2D.Impulse);

            StopCoroutine(nameof(OffDamaged_co));
            StartCoroutine(nameof(OffDamaged_co));

        }
    }

    protected abstract void PlaySound(string action);

    IEnumerator OffDamaged_co()
    {
        // �����ð� ��� �� ���� ���·� ���ƿ�
        yield return invincibleTime_wait;
        isImmobile = false;
        gameObject.layer = myLayer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = myLayer;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    // �ڽ��� ����ִ� ���� �ٸ� ������ ���� ���
    IEnumerator CheckWall_co()
    {
        // ����ĳ��Ʈ ���� ����
        int dir = transform.localScale.x < 0 ? 1 : -1;
        // �� ���̾� �̸�
        string[] platLayer = new string[] { "Middle", "Bottom"};
        // ĳ���� �ٷξ��� �浹�� ������ �ڽ�ĳ��Ʈ
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + dir * 0.5f * Vector3.right, new Vector2(0.3f, 1f), 0f,
                                                    dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(platLayer));

        // ���� �ֱ� ��Ҵ� �ٴ��� �±� �̸��� ĳ���� ���� �� ���̾� �̸��� ��ġ�ϸ�
        // ���� �浹�ؾ���
        if (raycastHit.collider != null && raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)) &&
            isGrounded)
        {
            Debug.Log(1);
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, false);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, false);
        }
        // �±׿� �� ���̾� �̸��� ��ġ���� ������
        // ���� �浹�ϸ� �ȵ�
        else if (raycastHit.collider != null && !raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)))
        {
            Debug.Log(2);
            Physics2D.IgnoreLayerCollision(myLayer, raycastHit.collider.gameObject.layer, true);
            Physics2D.IgnoreLayerCollision(invincibleLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }


    protected abstract void Attack();
    protected abstract void Move();
    protected abstract void GroundAct();

    // ���信�� ����ĳ��Ʈ �׸�
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
            int dir = transform.localScale.x < 0 ? 1 : -1;
            Vector2 boxCastSize2 = new Vector2(0.3f, 1f);
            Vector3 boxCastOrigin = transform.position + dir * 0.5f * Vector3.right;
            Gizmos.DrawWireCube(boxCastOrigin, boxCastSize2);
        }

    }
}
