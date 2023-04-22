using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int attack;
    public int defense;
    public float speed;
    public float jumpForce;


    public Vector3 playerPosition;
    public int strong;
    public int intelligence;
    public int dexterity;
    public int luck;
    public int exp;
    public int gold;
    public Define.Scene scene;
    public Inventory inventory;
}
