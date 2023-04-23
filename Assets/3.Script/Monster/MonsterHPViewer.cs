using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPViewer : MonoBehaviour
{
    [SerializeField]
    private Slider sliderHP;
    [SerializeField]
    private MonsterStat enemy;

    public void SetUp(MonsterStat enemy)
    {
        this.enemy = enemy;
        TryGetComponent(out sliderHP);
    }

    void Update()
    {
        if (!enemy.Equals(null))
        {
            sliderHP.value = enemy.Hp / (float)enemy.MaxHp;
        }
    }
}
