using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int level;
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected int maxHp;
    [SerializeField]
    protected int attack;
    [SerializeField]
    protected int defense;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float jumpForce;

    public int Level { get => level; set => level = value; }
    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp < 0)
            {
                hp = 0;
            }
            else if(hp > maxHp)
            {
                hp = maxHp;
            }
        }
    }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Atk { get => attack; set => attack = value; }
    public int Def { get => defense; set => defense = value; }
    public float Speed { get => speed; set => speed = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
}
