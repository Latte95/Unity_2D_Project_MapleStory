using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBtn : MonoBehaviour
{
    public void StatUp()
    {
        if(GameManager.Instance.nowPlayer.AbilityPoint<1)
        {
            return;
        }
        GameManager.SoundManager.PlaySfx(Define.Ui.Click);
        string parentName = transform.parent.name;
        switch (parentName)
        {
            case "Str":
                GameManager.Instance.nowPlayer.Str++;
                break;
            case "Dex":
                GameManager.Instance.nowPlayer.Dex++;
                break;
            case "Int":
                GameManager.Instance.nowPlayer.Int++;
                GameManager.Instance.nowPlayer.Mp += 10;
                break;
            case "Luk":
                GameManager.Instance.nowPlayer.Luk++;
                break;
        }
        GameManager.Instance.nowPlayer.AbilityPoint--;
    }
}
