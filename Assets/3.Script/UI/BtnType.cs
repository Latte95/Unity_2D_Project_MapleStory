using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public class BtnType : MonoBehaviour
{
    public BTNType currentType;

    public void OnBtnClick()
    {
        switch(currentType)
        {
            case BTNType.Login:

                break;
            case BTNType.Yes:

                break;
            case BTNType.Back:

                break;
            case BTNType.Exit:
                Application.Quit();
                break;
            case BTNType.NextScene:

                break;
            case BTNType.Dice:
                Debug.Log("¡÷ªÁ¿ß");
                RollringDice();
                break;
            case BTNType.Play:
                SceneManagerEX.LoadScene(Define.Scene.Henesys_Town);
                break;
        }
    }

    private void RollringDice()
    {
        int Str = 4;
        int Int = 4;
        int Dex = 4;
        int Luk = 4;
    }
}
