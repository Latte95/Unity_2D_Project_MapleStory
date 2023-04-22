using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    private Text Level;
    private Text Hp;
    private Text Mp;
    private Text Exp;
    private Text Str;
    private Text Dex;
    private Text Int;
    private Text Luk;
    private Text AbilityPoint;

    private void Awake()
    {
        Level = transform.Find("Level").GetComponent<Text>();
        Hp = transform.Find("Hp").GetComponent<Text>();
        Mp = transform.Find("Mp").GetComponent<Text>();
        Exp = transform.Find("Exp").GetComponent<Text>();
        Str = transform.Find("Str").GetComponent<Text>();
        Dex = transform.Find("Dex").GetComponent<Text>();
        Int = transform.Find("Int").GetComponent<Text>();
        Luk = transform.Find("Luk").GetComponent<Text>();
        AbilityPoint = transform.Find("AbilityPoint").GetComponent<Text>();
    }

    private void OnEnable()
    {
        Player player = GameManager.Instance.nowPlayer;
        Level.text = player.Level.ToString();
        Hp.text = player.MaxHp.ToString();
        Mp.text = player.MaxMp.ToString();
        Exp.text = player.Exp.ToString() + "/" + player.LevelUpExp.ToString();
        Str.text = player.Str.ToString();
        Dex.text = player.Dex.ToString();
        Int.text = player.Int.ToString();
        Luk.text = player.Luk.ToString();
        AbilityPoint.text = player.AbilityPoint.ToString();
    }
}
