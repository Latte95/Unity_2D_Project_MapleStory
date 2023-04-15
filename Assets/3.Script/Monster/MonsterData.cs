using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterData : Stat
{
    protected int exp;
    protected int gold;

    public int Exp { get => exp; set => exp = value; }
    public int Gold { get => gold; set => gold = value; }

    public void Init()
    {
        level = 1;
        maxHp = 20;
        hp = maxHp;
        attack = 3;
        defense = 2;
        speed = 0.9f;
        jumpForce = 20f;
        exp = 0;
        gold = 1000;
    }
}
