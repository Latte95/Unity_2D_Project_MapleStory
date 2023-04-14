using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BtnType : MonoBehaviour
{
    public BTNType currentType;
    private Animator anim;
    private Vector3 cameraPosition;
    [SerializeField]
    private GameObject UI;

    private void Awake()
    {
        TryGetComponent(out anim);
        cameraPosition = Camera.main.transform.position;
    }

    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.Login:
                Login();
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
                RollringDice();
                break;
            case BTNType.Play:
                SceneManagerEX.LoadScene(Define.Scene.HenesysTown);
                break;
        }
    }

    private void RollringDice()
    {
        anim.SetTrigger("Selected");
        int _str = 4;
        int _int = 5;
        int _dex = 6;
        int _luk = 7;
        Debug.Log($"{_str}, {_int}, {_dex}, {_luk} ");
    }

    private void Login()
    {
        transform.parent.gameObject.SetActive(false);
        SetCameraPosition(20);
        UIActive("CharacterSellect");
    }

    private void SetCameraPosition(int y)
    {
        Camera.main.transform.position = Vector3.MoveTowards(cameraPosition, cameraPosition + new Vector3(0, y, 0), y);
    }

    private void UIActive(string name)
    {
        Transform parant = GameObject.Find(name).transform;
        foreach (Transform child in parant)
        {
            child.gameObject.SetActive(true);
        }
    }
}
