using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : ControlManager
{
    protected override void Attack()
    {
    }

    protected override void Move()
    {
        //StartCoroutine(nameof(CheckWall_co));
        // �̵�
        // ������ �ְų� �ȴ� �߿��� �̵� ����, �ǰ����� �� �̵� �Ұ���
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(MoveAni)) &&
            !isImmobile)
        {
            movement.MoveTo(Vector3.left);
            //movement.MoveTo(Vector3.right);
            //movement.MoveTo(Vector3.zero);
        }
        // �����߿��� �̵� �Ұ���
        // => �������� �ƴҶ��� �̵� ����
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            movement.MoveTo(Vector3.zero);
        }
    }

    protected override void GroundAct()
    {
        if (isGrounded)
        {
            //movement.JumpTo();
        }
    }
}
