using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEffect : MonoBehaviour
{
    public AnimatorOverrideController attack;
    public AnimatorOverrideController magicClaw;

    public void DarkSkin()
    {
        GetComponent<Animator>().runtimeAnimatorController = attack;
    }

    public void WhiteSkin()
    {
        GetComponent<Animator>().runtimeAnimatorController = magicClaw;
    }
}
