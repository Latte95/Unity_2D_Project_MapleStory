using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Movement movement;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rigid;

    private Vector2 boxCastSize = new Vector2(0.3f, 0.05f);
    private float boxCastMaxDistance = 0.05f;

    private float invincibleTime = 2f;

    [SerializeField]
    private bool isGrounded = false;
    [SerializeField]
    private bool isSlope = false;
    public bool IsSlope => isSlope;
    //private bool isBottom = false;
    [SerializeField]
    private bool isImmobile = false;
    [SerializeField]
    private string lastGroundTag;

    float deltaTime;
    private WaitForSeconds invincibleTime_wait;
    private WaitForSeconds ignorePlatTime_wait;
    private Transform HeadTrans;
    private Transform FootTrans;

    private int playerLayer;
    private int invincibleLayer;
    private int groundLayer;

    private void Awake()
    {
        TryGetComponent(out movement);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out anim);
        TryGetComponent(out rigid);

        invincibleTime_wait = new WaitForSeconds(invincibleTime);
        ignorePlatTime_wait = new WaitForSeconds(1f);
        deltaTime = Time.deltaTime;
    }

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        invincibleLayer = LayerMask.NameToLayer("Invincible");
        groundLayer = LayerMask.NameToLayer("Ground");
        HeadTrans = transform.GetChild(0);
        FootTrans = transform.GetChild(1);
    }

    void Update()
    {
        // 땅 밟고 있는지 체크
        isGrounded = IsOnGround();

        // 비탈길에 있을 경우 미끄러짐 방지
        if (isSlope)
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 공격
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(nameof(Attack_co));
        }

        Move();

        // 땅에서
        if (isGrounded)
        {
            Prone();
            if (Input.GetButtonDown("Jump"))
            {
                // 서있을 때 점프
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
                {
                    movement.JumpTo();
                }
                // 누워 있을 때 점프 -> 아래 점프
                else
                {
                    StartCoroutine(nameof(DownJump_co));
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            OnDamaged(col.gameObject.transform.position);
        }
    }

    IEnumerator Attack_co()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Nomal01") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            anim.SetBool("isAttack", true);
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("isAttack", false);
        }
    }

    private bool IsOnGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(FootTrans.position, boxCastSize, 0f, Vector2.down,
                                                    boxCastMaxDistance, LayerMask.GetMask("Ground", "Slope"));
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

    private void Move()
    {
        StartCoroutine(nameof(CheckWall_co));
        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능, 피격중일 땐 이동 불가능
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Nomal01")) &&
            !isImmobile)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.MoveTo(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.MoveTo(Vector3.right);
            }
            else
            {
                movement.MoveTo(Vector3.zero);
            }
        }
        // 점프중에는 이동 불가능
        // => 점프중이 아닐땐 이동 멈춤
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(Vector3.zero);
        }
    }

    private void OnDamaged(Vector2 targetPos)
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
        gameObject.layer = playerLayer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = playerLayer;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    IEnumerator DownJump_co()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, LayerMask.NameToLayer(lastGroundTag), true);
        yield return ignorePlatTime_wait;
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, LayerMask.NameToLayer(lastGroundTag), false);
    }

    IEnumerator CheckWall_co()
    {
        int dir = spriteRenderer.flipX ? 1 : -1;
        string[] str = new string[] { "Middle", "Bottom" };
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + dir*0.5f*Vector3.right, new Vector2(0.2f, 1f), 0f, dir * Vector2.right, boxCastMaxDistance, LayerMask.GetMask(str));
        if (raycastHit.collider != null && raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag))&&
            anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Nomal01"))
        {
            Physics2D.IgnoreLayerCollision(playerLayer, raycastHit.collider.gameObject.layer, false);
        }
        else if (raycastHit.collider != null && !raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(lastGroundTag)))
        {
            Physics2D.IgnoreLayerCollision(playerLayer, raycastHit.collider.gameObject.layer, true);
        }

        yield return null;
    }

    // 레이캐스트
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

    public void Prone()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("isDown", true);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            anim.SetBool("isDown", false);
        }
    }
}
