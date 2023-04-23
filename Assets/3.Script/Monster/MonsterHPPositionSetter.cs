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
        // ������Ʈ ��ġ�� ���ŵǸ� UI�� ���󰡾ߵȴ�.
        // �ٵ� UI�� canvas���� ��� ����
        // => ���� ī�޶� ��ġ���� ������Ʈ�� ��ġ�� ã��
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(Target.transform.position);

        UITrans.position = ScreenPosition + distance;
    }
}
