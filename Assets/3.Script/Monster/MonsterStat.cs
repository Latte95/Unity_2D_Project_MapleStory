using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : Stat
{
    [SerializeField]
    protected int exp;
    [SerializeField]
    protected int gold;

    public int Exp { get => exp; set => exp = value; }
    public int Gold { get => gold; set => gold = value; }

    public void Init()
    {
        hp = maxHp;
    }
}
