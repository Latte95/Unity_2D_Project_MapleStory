using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : CreatureControl
{
    protected AudioClip audioHit;
    private MonsterStat Stat;

    private new void OnEnable()
    {
        base.OnEnable();
        TryGetComponent(out audioHit);
        TryGetComponent(out Stat);
        Stat.Init();
    }
    private new void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {

    }

    protected override void Move()
    {
        //StartCoroutine(nameof(CheckWall_co));
        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����, �ǰ����� �� �̵� �Ұ���
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(walkAni)) &&
            !isImmobile)
        {
            movement.MoveTo(Vector2.left);
            //movement.MoveTo(Vector2.right);
            //movement.MoveTo(Vector2.zero);
        }
        // �����߿��� �̵� �Ұ���
        // => �������� �ƴҶ��� �̵� ����
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(Vector2.zero);
        }
    }

    protected override void GroundAct()
    {
        if (isGrounded)
        {
            //movement.JumpTo();
        }
    }

    public override void OnDamaged(Vector2 targetPos, int damage)
    {
        if (!isImmobile)
        {
            base.OnDamaged(targetPos, damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl player))
        {
            player.OnDamaged(transform.position, Stat.Atk);
        }
    }

    protected override IEnumerator OffDamaged_co()
    {
        // �����ð� ��� �� ���� ���·� ���ƿ�
        yield return invincibleTime_wait;
        isImmobile = false;
    }

}
