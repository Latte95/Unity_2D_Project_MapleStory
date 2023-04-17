using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    private static GameManager Instance;

    public GameObject player;
    public SoundManager soundManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
    }

    private void Start()
    {
        DataManager.instance.LoadGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            OnSaveGameClicked();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name.Equals(nameof(Define.Scene.HenesysTown)))
            {
                StartCoroutine(LoadSceneAndData(Define.Scene.HenesysField));
            }
            else if (scene.name.Equals(nameof(Define.Scene.HenesysField)))
            {
                StartCoroutine(LoadSceneAndData(Define.Scene.HenesysTown));
            }

        }
    }

    public void LoadData(Player data)
    {
        player.transform.position = data.playerPosition;
    }
    public void SaveData(ref Player data)
    {
        data.playerPosition = player.transform.position;
    }

    public void OnSaveGameClicked()
    {
        DataManager.instance.SaveGame();
    }
    public void GameExit()
    {
        Application.Quit();
    }
    public static void Clear()
    {
        //Sound.Clear();
        //Scene.Clear();
        //UI.Clear();
    }

    IEnumerator LoadSceneAndData(Define.Scene targetScene)
    {
        // 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene.ToString());

        // 씬 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        player = GameObject.Find("Player");
        soundManager = FindObjectOfType<SoundManager>();
        // 데이터 로드
        DataManager.instance.LoadGame();
    }
}
