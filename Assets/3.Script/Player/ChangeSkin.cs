using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkin : MonoBehaviour
{
    public AnimatorOverrideController darkAnim;
    public AnimatorOverrideController whiteAnim;

    public void DarkSkin()
    {
        GetComponent<Animator>().runtimeAnimatorController = darkAnim as RuntimeAnimatorController;
    }

    public void WhiteSkin()
    {
        GetComponent<Animator>().runtimeAnimatorController = whiteAnim as RuntimeAnimatorController;
    }
}
