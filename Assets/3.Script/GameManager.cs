using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private StageData stageData;

    public static GameManager Instance;
    private MainUI _ui;
    public static MainUI UI => Instance._ui;

    public GameObject player;
    public GameObject canvasPrefab;
    public GameObject playerPrefab;
    public GameObject soundPrefab;
    public Player nowPlayer;
    public SoundManager soundManager;
    private Portal portal;
    private Image fadeImage;

    private WaitUntil inputUpArrow_wait;

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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        DataManager.instance.LoadGame();

        if (nowPlayer == null)
        {
            nowPlayer = player.GetComponent<Player>();
        }
        FadeOutAndLoadScene(nowPlayer.Scene);
        inputUpArrow_wait = new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow));
        StartCoroutine(nameof(InputUpArrow_co));
    }

    private void Update()
    {
        // 세이브
        if (Input.GetKeyDown(KeyCode.F11))
        {
            FindObjectOfType<QuickSlotManager>().SaveData(ref nowPlayer);
            _ui.EquipUI.SetActive(true);
            FindObjectOfType<EquipUI>().SaveData(ref nowPlayer);
            _ui.EquipUI.SetActive(_ui.activeEquip);
            DataManager.instance.SaveGame();
        }
        // 데이터 초기화
        if (Input.GetKeyDown(KeyCode.F12))
        {
            nowPlayer.Init(4, 4, 4, 4);
            DataManager.instance.SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlaySfx(Define.Sfx.Transform);
            player.transform.position += player.transform.localScale.x * 3 * Vector3.left;
        }

        // 장비 획득
        if (Input.GetKeyDown(KeyCode.F6))
        {
            nowPlayer.inventory.GetItem("검");
            nowPlayer.inventory.GetItem(01040002, 2);
            nowPlayer.inventory.GetItem(1060002);
        }
    }

    // 플레이어 위치 저장 및 로드
    public void LoadData(Player data)
    {
        player.transform.position = data.playerPosition;
        //string currentSceneName = SceneManager.GetActiveScene().name;
    }
    public void SaveData(ref Player data)
    {
        data.playerPosition = player.transform.position;
        string currentSceneName = SceneManager.GetActiveScene().name;
        data.Scene = (Define.Scene)Enum.Parse(typeof(Define.Scene), currentSceneName);
    }

    // 클릭 이벤트
    public void OnSaveGameClicked()
    {
        DataManager.instance.SaveGame();
    }
    public void GameExit()
    {
        DataManager.instance.SaveGame();
        Application.Quit();
    }

    public void LoadSceneAndData(Define.Scene targetScene)
    {
        if (portal != null)
        {
            player.transform.position = portal.position;
        }
        // 플레이어 데이터 저장
        DataManager.instance.SaveGame();

        // 씬 로드가 완료되면 실행될 메서드를 지정
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 씬 로드
        SceneManager.LoadScene(targetScene.ToString());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Canvas 할당
        MainUI existingMainUI = FindObjectOfType<MainUI>();
        if (existingMainUI == null)
        {
            // Instantiate the Canvas prefab and assign the MainUI component to the _ui field
            GameObject canvasInstance = Instantiate(canvasPrefab);
            _ui = canvasInstance.GetComponent<MainUI>();

            // Set the name of the instantiated object to match the original prefab
            canvasInstance.name = canvasPrefab.name;
            fadeImage = canvasInstance.transform.Find("Fade").GetComponent<Image>();
        }
        else
        {
            _ui = existingMainUI;
        }
        // soundManager 할당
        if (soundManager == null)
        {
            GameObject soundInstance = Instantiate(soundPrefab);
            soundManager = FindObjectOfType<SoundManager>();
        }

        // StageData 할당
        if (stageData == null)
        {
            stageData = Resources.Load<StageData>("StageData/" + nowPlayer.Scene.ToString());
        }

        // Player 컴포넌트 할당
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        if (nowPlayer == null)
        {
            player.TryGetComponent(out nowPlayer);
        }

        // CursorManager에 transform_cursor 할당
        CursorManager cursorManager = GetComponent<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.Init_Cursor();
        }

        // Main Camera 설정
        CameraControl cameraControl = FindObjectOfType<CameraControl>();

        // 플레이어 데이터 로드
        DataManager.instance.LoadGame();

        DataManager.instance.SaveGame();
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(nameof(FadeIn));
    }

    public void FadeOutAndLoadScene(Define.Scene targetScene)
    {
        // Instantiate the Canvas prefab and assign the MainUI component to the _ui field
        GameObject canvasInstance = Instantiate(canvasPrefab);
        _ui = canvasInstance.GetComponent<MainUI>();

        // Set the name of the instantiated object to match the original prefab
        canvasInstance.name = canvasPrefab.name;
        fadeImage = canvasInstance.transform.Find("Fade").GetComponent<Image>();
        fadeImage.gameObject.SetActive(true);
        // Fade out
        Color newColor = fadeImage.color;
        newColor.a = 1;
        fadeImage.color = newColor;

        // Load scene
        LoadSceneAndData(targetScene);
    }
    private IEnumerator FadeIn()
    {
        // Fade in
        for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime)
        {
            Color newColor = fadeImage.color;
            newColor.a = alpha;
            fadeImage.color = newColor;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
    private IEnumerator InputUpArrow_co()
    {
        while (true)
        {
            yield return inputUpArrow_wait;

            int layerMask = LayerMask.GetMask("Portal");
            Collider2D col = Physics2D.OverlapCircle(player.transform.position, 0.5f, layerMask);

            // Portal 감지하면 씬로드
            if (col != null)
            {
                soundManager.PlaySfx(Define.Sfx.Portal);
                col.TryGetComponent(out portal);
                FadeOutAndLoadScene(portal.scene);
            }
        }
    }
}
