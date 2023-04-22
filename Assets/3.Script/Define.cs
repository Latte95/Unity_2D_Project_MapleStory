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
        Title, HenesysTown, HenesysField
    }

    public enum Sound
    {
        BgmPlayer, SfxPlayer, SoundCount
    }

    public enum Sfx
    {
        Click, Over, Buff, Dead, Jump, LevelUp, PickUpItem, Portal, 
        QuestAlert, QuestClear, Transform, UseItem, AttackS, Magic,
    }

    public enum Ui
    {
        Click, Over
    }

    public enum Skill
    {
        Attack, MagicClaw,
    }
}
