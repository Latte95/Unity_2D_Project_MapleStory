using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NomalBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    private void Awake()
    {
        TryGetComponent(out button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.SoundManager.PlaySfx(Define.Ui.Over);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null)
        {
            button.OnDeselect(null);
        }
    }
}
