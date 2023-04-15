using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        // Start 되기전에 다른곳에서 호출되면 실행하기 위함
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
        // 유일성 보장
        if (instance == null)
        {
            // 게임 매니저를 찾아서 없으면 오브젝트 생성하고 스크립트 붙이기
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
