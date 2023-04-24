using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : Stat
{
    [SerializeField]
    protected int exp;

    public int Exp { get => exp; set => exp = value; }

    public void Init()
    {
        hp = maxHp;
    }
}
