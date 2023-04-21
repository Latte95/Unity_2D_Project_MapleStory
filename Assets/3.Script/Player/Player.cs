using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Stat
{
    public Vector3 playerPosition;

    [SerializeField]
    protected int mp;
    [SerializeField]
    protected int maxMp;

    protected int strong;
    protected int intelligence;
    protected int dexterity;
    [SerializeField]
    protected int luck;

    [SerializeField]
    protected int exp;
    [SerializeField]
    protected int levelUpExp;
    [SerializeField]
    protected int gold;

    protected Define.Scene _scene;
    public Define.Scene scene { get => _scene; set => _scene = value; }
    public Inventory inventory;
    public int Mp
    {
        get => mp;
        set
        {
            mp = value;
            if (mp < 0)
            {
                mp = 0;
            }
            else if (mp > maxMp)
            {
                mp = maxMp;
            }
        }
    }
    public int MaxMp { get => maxMp; set => maxMp = value; }

    public int Str { get => strong; set => strong = value; }
    public int Int { get => intelligence; set => intelligence = value; }
    public int Dex { get => dexterity; set => dexterity = value; }
    public int Luk { get => luck; set => luck = value; }

    public int Exp
    {
        get => exp;
        set
        {
            exp = value;
            if (exp < 0)
            {
                exp = 0;
            }
        }
    }
    public int LevelUpExp { get => levelUpExp; }
    public int Gold { get => gold; set => gold = value; }

    public Define.Scene Scene { get => scene; set => scene = value; }

    private WaitUntil leverUp_wait;


    public GameObject levelUpEffect;


    public PlayerData ToPlayerData()
    {
        PlayerData data = new PlayerData();
        data.level = level;
        data.hp = hp;
        data.maxHp = maxHp;
        data.mp = mp;
        data.maxMp = maxMp;
        data.attack = attack;
        data.defense = defense;
        data.speed = speed;
        data.jumpForce = jumpForce;
        data.playerPosition = playerPosition;
        data.strong = strong;
        data.intelligence = intelligence;
        data.dexterity = dexterity;
        data.luck = luck;
        data.exp = exp;
        data.gold = gold;
        data.scene = scene;
        data.inventory = inventory;
        return data;
    }

    private void Awake()
    {
        Init(4, 4, 4, 4);
        levelUpExp = 100 + (level * level + 10);
        leverUp_wait = new WaitUntil(() => exp >= levelUpExp);
        StartCoroutine(nameof(LevelUp_co));
    }
    public void SetData(PlayerData data)
    {
        level = data.level;
        maxHp = data.maxHp;
        hp = data.hp;
        maxMp = data.maxMp;
        mp = data.mp;
        attack = data.attack;
        defense = data.defense;
        speed = data.speed;
        jumpForce = data.jumpForce;
        exp = data.exp;
        gold = data.gold;
        scene = data.scene;

        strong = data.strong;
        intelligence = data.intelligence;
        dexterity = data.dexterity;
        luck = data.luck;

        playerPosition = data.playerPosition;

        inventory = data.inventory;
        foreach (Item item in inventory.items)
        {
            item.itemIcon = Resources.Load<Sprite>("ItemIcon/" + item._itemID.ToString());
        }
    }

    public void Init(int _str, int _int, int _dex, int _luk)
    {
        level = 1;
        maxHp = 100;
        hp = maxHp;
        maxMp = 100;
        mp = maxMp;
        attack = 10;
        defense = 5;
        speed = 1.8f;
        jumpForce = 25f;
        exp = 0;
        gold = 1000;
        scene = Define.Scene.HenesysField;

        strong = _str;
        intelligence = _int;
        dexterity = _dex;
        luck = _luk;
        inventory.items.Clear();
    }

    public event Action LevelUp;
    private IEnumerator LevelUp_co()
    {
        while (true)
        {
            yield return leverUp_wait;
            SoundManager.Instance.PlaySfx(Define.Sfx.LevelUp);
            level++;
            exp += -levelUpExp;
            levelUpExp = 100 + (level * level + 10);
            levelUpEffect.SetActive(true);
            StartCoroutine(nameof(EffectOff_co));

            LevelUp?.Invoke();
        }
    }
    private IEnumerator EffectOff_co()
    {
        yield return new WaitForSeconds(1.8f);
        levelUpEffect.SetActive(false);
    }
}
