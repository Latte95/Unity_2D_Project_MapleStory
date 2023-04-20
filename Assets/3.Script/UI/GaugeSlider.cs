using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeSlider : MonoBehaviour
{
    private Slider slider;
    private Text text;
    int currentValue, maxValue;

    private void Start()
    {
        TryGetComponent(out slider);
        text = transform.Find("Text").GetComponent<Text>();
    }

    void Update()
    {
        switch(gameObject.name)
        {
            case "HP":
                currentValue = GameManager.Instance.nowPlayer.Hp;
                maxValue = GameManager.Instance.nowPlayer.MaxHp;
                break;
            case "MP":
                currentValue = GameManager.Instance.nowPlayer.Mp;
                maxValue = GameManager.Instance.nowPlayer.MaxMp;
                break;
            case "Exp":
                currentValue = GameManager.Instance.nowPlayer.Exp;
                maxValue = GameManager.Instance.nowPlayer.LevelUpExp;
                break;
        }

        slider.value = currentValue / (float)maxValue;
        text.text = $"[{currentValue} / {maxValue}]";
    }
}
