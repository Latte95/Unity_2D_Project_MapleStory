using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    public BTNType currentType;
    private Animator anim;
    [SerializeField]
    SoundManager sound;
    [SerializeField]
    private Vector3 cameraPosition;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out button);
        cameraPosition = Camera.main.transform.position;
        sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    public void OnBtnClick()
    {
        sound.PlaySfx(Define.Ui.Click);
        switch (currentType)
        {
            case BTNType.Login:
                SetCameraPosition(20);
                break;
            case BTNType.Yes:
                SetCameraPosition(-10);
                break;
            case BTNType.Back:
                SetCameraPosition(-10);
                break;
            case BTNType.Exit:
                Application.Quit();
                break;
            case BTNType.BackToFirst:
                BackToFirst();
                break;
            case BTNType.Dice:
                RollringDice();
                break;
            case BTNType.Play:
                SceneManager.LoadScene(Define.Scene.HenesysTown.ToString());
                break;
            case BTNType.Create:
                SetCameraPosition(10);
                break;
            case BTNType.Item:
                SetCameraPosition(10);
                break;
        }
    }

    private void RollringDice()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Selected"))
        {
            anim.SetTrigger("Selected");
            int _str = 4;
            int _int = 5;
            int _dex = 6;
            int _luk = 7;
            Debug.Log($"{_str}, {_int}, {_dex}, {_luk} ");
        }
    }

    private void SetCameraPosition(int y)
    {
        Camera.main.transform.position = Vector3.MoveTowards(cameraPosition, cameraPosition + new Vector3(0, y, 0), Mathf.Abs(y));
    }

    private void BackToFirst()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sound.PlaySfx(Define.Ui.Over);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null)
        {
            button.OnDeselect(null);
        }
    }
}
