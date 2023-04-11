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
        // 이동
        // 가만히 있거나 걷는 중에만 이동 가능, 피격중일 땐 이동 불가능
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName(MoveAni)) &&
            !isImmobile)
        {
            movement.MoveTo(Vector3.left);
            //movement.MoveTo(Vector3.right);
            //movement.MoveTo(Vector3.zero);
        }
        // 점프중에는 이동 불가능
        // => 점프중이 아닐때만 이동 멈춤
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
