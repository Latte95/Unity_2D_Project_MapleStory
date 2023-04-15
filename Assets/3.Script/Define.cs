using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum MoveDirection
    {
        None, Left, Right
    }

    public enum Scene
    {
        Login, HenesysTown, Henesys_Field
    }

    public enum Sound
    {
        BgmPlayer, SfxPlayer, SoundCount
    }

    public enum Sfx
    {
        AttackS, AttackL, Buff, Dead, Jump, LevelUp, PickUpItem, Portal, QuestAlert, QuestClear, Transform, UseItem
    }

    public enum Ui
    {
        Click, Over
    }
}
