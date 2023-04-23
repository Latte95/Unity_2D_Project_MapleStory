using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHPPositionSetter : MonoBehaviour
{
    public GameObject Target;
    private RectTransform UITrans;

    private Vector3 distance = 70 * Vector3.up;

    public void SetUp(GameObject target)
    {
        this.Target = target;
        TryGetComponent(out UITrans);
    }

    private void LateUpdate()
    {
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
