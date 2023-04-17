using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Stat
{
    public Vector3 playerPosition;
    protected int strong;
    protected int intelligence;
    protected int dexterity;
    protected int luck;

    [SerializeField]
    protected int exp;
    [SerializeField]
    protected int levelUpExp;
    [SerializeField]
    protected int gold;

    protected Define.Scene scene;
    public Inventory inventory;
    private InventoryUI inventoryUI;

    public int Str { get => strong; set => strong = value; }
    public int Int { get => intelligence; set => intelligence = value; }
    public int Dex { get => dexterity; set => dexterity = value; }
    public int Luk { get => luck; set => luck = value; }

    public int Exp { get => exp; set => exp = value; }
    public int Gold { get => gold; set => gold = value; }

    public Define.Scene Scene { get => scene; set => scene = value; }


    private WaitUntil leverUp_wait;
    public Player(int _str, int _int, int _dex, int _luk)
    {
        this.playerPosition = Vector3.zero;
        level = 1;
        maxHp = 100;
        hp = maxHp;
        attack = 10;
        defense = 5;
        speed = 1.8f;
        jumpForce = 23f;
        exp = 0;
        gold = 1000;
        scene = Define.Scene.HenesysField;

        strong = _str;
        intelligence = _int;
        dexterity = _dex;
        luck = _luk;

        inventory = new Inventory();
    }

    public PlayerData ToPlayerData()
    {
        PlayerData data = new PlayerData();
        data.level = level;
        data.hp = hp;
        data.maxHp = maxHp;
        data.attack = attack;
        data.defense = defense;
        data.speed = speed;
        data.jumpForce = jumpForce;
        data.playerPosition = playerPosition;
        data.strong = strong;
        data.intelligence = intelligence;
        data.dexterity = dexterity;
        data.exp = exp;
        data.gold = gold;
        data.scene = scene;
        data.inventory = inventory;
        data.inventoryUI = inventoryUI;

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
        //inventoryUI.SetInventory(inventory);
    }

    public void Init(int _str, int _int, int _dex, int _luk)
    {
        level = 1;
        maxHp = 100;
        hp = maxHp;
        attack = 10;
        defense = 5;
        speed = 1.8f;
        jumpForce = 23f;
        exp = 0;
        gold = 1000;
        scene = Define.Scene.HenesysField;

        strong = _str;
        intelligence = _int;
        dexterity = _dex;
        luck = _luk;

        inventory = new Inventory();
        //inventoryUI.SetInventory(inventory);
    }

    private IEnumerator LevelUp_co()
    {
        while (true)
        {
            yield return leverUp_wait;
            SoundManager.instance.PlaySfx(Define.Sfx.LevelUp);
            level++;
            exp += -levelUpExp;
            levelUpExp = 100 + (level * level + 10);
        }
    }
}
