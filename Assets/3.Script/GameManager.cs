using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        // Start �Ǳ����� �ٸ������� ȣ��Ǹ� �����ϱ� ����
        get
        {
            Init();
            return instance;
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
    }

    static void Init()
    {
        // ���ϼ� ����
        if (instance == null)
        {
            // ���� �Ŵ����� ã�Ƽ� ������ ������Ʈ �����ϰ� ��ũ��Ʈ ���̱�
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)
            {
                gm = new GameObject { name = "GameManager" };
                gm.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(gm);
            gm.TryGetComponent(out instance);
        }
    }
    public static void Clear()
    {
        //Sound.Clear();
        //Scene.Clear();
        //UI.Clear();
    }
}
