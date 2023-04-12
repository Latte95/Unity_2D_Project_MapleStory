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
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(moveAni)) &&
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

    private new void OnDamaged(Vector2 targetPos)
    {
        base.OnDamaged(targetPos);
    }

    protected override void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                //audioSource.clip = audioJump;
                break;
            case "Hit":
                audioSource.clip = audioHit;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerStat player))
        {
            int damage = 1;
            if(Stat.Atk - player.Def > 1)
            {
                damage = Stat.Atk - player.Def;
            }
            player.Hp -= damage;
        }
    }

}