using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager Instance;
    public MainUI _ui;
    public static MainUI UI => Instance._ui;

    public GameObject canvasPrefab;
    public GameObject player;
    public GameObject playerPrefab;
    public Player nowPlayer;
    public SoundManager soundManager;
    //public DataManager dataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            MainUI existingMainUI = FindObjectOfType<MainUI>();
            if (existingMainUI == null)
            {
                // Instantiate the Canvas prefab and assign the MainUI component to the _ui field
                GameObject canvasInstance = Instantiate(canvasPrefab);
                _ui = canvasInstance.GetComponent<MainUI>();

                // Set the name of the instantiated object to match the original prefab
                canvasInstance.name = canvasPrefab.name;
            }
            else
            {
                _ui = existingMainUI;
            }
            player = GameObject.FindGameObjectWithTag("Player");
            if(player == null)
            {
                GameObject playerInstance = Instantiate(playerPrefab);
                player = playerInstance;
            }
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        DataManager.instance.LoadGame();
        player.TryGetComponent(out nowPlayer);
    }

    private void OnEnable()
    {
        //nowPlayer = player.GetComponent<Player>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        // ���̺�
        if (Input.GetKeyDown(KeyCode.F11))
        {
            OnSaveGameClicked();
        }
        // ������ �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.F12))
        {
            nowPlayer.Init(4,4,4,4);
        }
        // ���̵�
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

        // ������ ȹ��
        if (Input.GetKeyDown(KeyCode.F5))
        {
            nowPlayer.inventory.GetItem(02000000,2);
            nowPlayer.inventory.GetItem("��Ȳ ����");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            nowPlayer.inventory.GetItem("��");
            nowPlayer.inventory.GetItem(01040002,2);
            nowPlayer.inventory.GetItem(1060002);
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
        OnSaveGameClicked();
        // �� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene.ToString());

        // �� �ε尡 �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        MainUI existingMainUI = FindObjectOfType<MainUI>();

        if (existingMainUI == null)
        {
            // Instantiate the Canvas prefab and assign the MainUI component to the _ui field
            GameObject canvasInstance = Instantiate(canvasPrefab);
            _ui = canvasInstance.GetComponent<MainUI>();

            // Set the name of the instantiated object to match the original prefab
            canvasInstance.name = canvasPrefab.name;
        }
        else
        {
            _ui = existingMainUI;
        }

        // player �Ҵ�
        player = GameObject.Find("Player");
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        nowPlayer = player.GetComponent<Player>();
        soundManager = FindObjectOfType<SoundManager>();

        // CursorManager�� transform_cursor �Ҵ�
        CursorManager cursorManager = GetComponent<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.Init_Cursor();
        }
        else
        {
            Debug.LogWarning("CursorManager not found on GameManager.");
        }

        // InventoryUI�� player �Ҵ�
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.InitializePlayer();
        }
        else
        {
            Debug.LogWarning("InventoryUI not found in the scene.");
        }

        CameraControl cameraControl = FindObjectOfType<CameraControl>();
        if(cameraControl != null)
        {
            cameraControl.InitializePlayer();
        }


        // ������ �ε�
        DataManager.instance.LoadGame();
        player.transform.position = Vector3.zero;
    }
}
