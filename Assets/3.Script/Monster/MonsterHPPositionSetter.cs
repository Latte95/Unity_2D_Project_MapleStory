using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHPPositionSetter : MonoBehaviour
{
    public GameObject Target;
    private RectTransform UITrans;
    public MonsterControl monster;

    private Vector3 distance = 70 * Vector3.up;

    public void SetUp(GameObject target)
    {
        this.Target = target;
        TryGetComponent(out UITrans);
        Target.TryGetComponent(out monster);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(monster.onHP);
        }
        if (!Target.activeSelf)
        {
            Destroy(gameObject);
            return;
        }
        // 오브젝트 위치가 갱신되면 UI도 따라가야된다.
        // 근데 UI는 canvas에게 상속 받음
        // => 현재 카메라 위치에서 오브젝트의 위치를 찾자
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(Target.transform.position);

        UITrans.position = ScreenPosition + distance;
    }
}
