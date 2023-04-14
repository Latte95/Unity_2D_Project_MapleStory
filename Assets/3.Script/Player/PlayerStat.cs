using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Stat
{
    protected int strong;
    protected int intelligence;
    protected int dexterity;
    protected int luck;

    protected int exp;
    protected int gold;

    protected Define.Scene scene;

    public int Str { get => strong; set => strong = value; }
    public int Int { get => intelligence; set => intelligence = value; }
    public int Dex { get => dexterity; set => dexterity = value; }
    public int Luk { get => luck; set => luck = value; }

    public int Exp { get => exp; set => exp = value; }
    public int Gold { get => gold; set => gold = value; }

    public Define.Scene Scene { get => scene; set => scene = value; }


    public void Init()
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
        scene = Define.Scene.Henesys_Field;
    }
}
