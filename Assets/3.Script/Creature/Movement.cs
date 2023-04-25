using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Animator anim;
    private Stat Stat;
    [SerializeField]
    private StageData stageData;

    private Vector2 moveDirection = Vector2.zero;

    float deltaTime;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out rigid);
        TryGetComponent(out anim);
        TryGetComponent(out Stat);

        deltaTime = Time.deltaTime;
    }

    private void Start()
    {
        StartCoroutine(nameof(WaitGameManager_co));
    }

    private void FixedUpdate()
    {
        // �̵� -> ������ٵ� �̿� : �ӵ����, ������ ����
        //rigid.position += Stat.Speed * deltaTime * moveDirection;
        rigid.velocity = new Vector3(Stat.Speed * moveDirection.x, rigid.velocity.y, 0);
        // ���� ����
        if (moveDirection.Equals(Vector2.left))
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 0); ;
            }
        }
        else if (moveDirection.Equals(Vector2.right))
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 0); ;
            }
        }

        if (stageData != null)
        {
            rigid.position = new Vector3(
                Mathf.Clamp(transform.position.x, stageData.LimitMin.x, stageData.LimitMax.x),
                rigid.position.y,
                0f);
            if (rigid.position.y < stageData.LimitMin.y)
            {
                rigid.position = Vector2.zero;
            }
        }
    }


    public void MoveTo(Vector2 h)
    {
        moveDirection = h;
        anim.SetInteger("h", (int)h.x);
    }

    public void JumpTo()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(Stat.JumpForce * Vector3.up, ForceMode2D.Impulse);
    }

    IEnumerator WaitGameManager_co()
    {
        yield return new WaitUntil(() => GameManager.Instance.player != null);
        string sceneName = GameManager.Instance.player.scene.name;
        stageData = Resources.Load<StageData>("StageData/" + sceneName);
    }
}