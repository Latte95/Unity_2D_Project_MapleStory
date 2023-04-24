using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    private StageData stageData;

    public static GameManager Instance;
    private MainUI _ui;
    public static MainUI UI => Instance._ui;
    private SoundManager soundManager;
    public static SoundManager SoundManager => Instance.soundManager;

    public GameObject player;
    [SerializeField]
    private GameObject canvasPrefab;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject soundPrefab;
    public Player nowPlayer;
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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.name = playerPrefab.name;
            player = playerInstance;
        }
        if (nowPlayer == null)
        {
            nowPlayer = player.GetComponent<Player>();
        }
        inputUpArrow_wait = new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow));
    }

    private void Start()
    {        
        DataManager.instance.LoadGame();

        FadeOutAndLoadScene(nowPlayer.Scene);
        StartCoroutine(nameof(InputUpArrow_co));
    }

    private void Update()
    {
        // 세이브
        if (Input.GetKeyDown(KeyCode.F11))
        {
            // 퀵슬롯 저장
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
        // 원활한 테스트 위한 텔레포트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlaySfx(Define.Sfx.Transform);
            player.transform.position += player.transform.localScale.x * 3 * Vector3.left;
        }
        // 원활한 테스트 위한 장비 획득
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

    // 지금 씬 종료
    public void FadeOutAndLoadScene(Define.Scene targetScene)
    {
        GameObject canvasInstance = Instantiate(canvasPrefab);
        canvasInstance.name = canvasPrefab.name;

        _ui = canvasInstance.GetComponent<MainUI>();
        fadeImage = canvasInstance.transform.Find("Fade").GetComponent<Image>();
        fadeImage.gameObject.SetActive(true);

        // 페이드아웃
        Color newColor = fadeImage.color;
        newColor.a = 1;
        fadeImage.color = newColor;

        // Load scene
        LoadSceneAndData(targetScene);
    }
    // 다음 씬 로드
    public void LoadSceneAndData(Define.Scene targetScene)
    {
        if (portal != null)
        {
            player.transform.position = portal.position;
        }
        // 플레이어 데이터 저장
        DataManager.instance.SaveGame();

        // 씬 로드가 완료되면 실행
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 씬 로드
        SceneManager.LoadScene(targetScene.ToString());
    }
    // 씬 로드 끝난 후
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Canvas 생성
        MainUI existingMainUI = FindObjectOfType<MainUI>();
        if (existingMainUI == null)
        {
            GameObject canvasInstance = Instantiate(canvasPrefab);
            canvasInstance.name = canvasPrefab.name;

            _ui = canvasInstance.GetComponent<MainUI>();
            fadeImage = canvasInstance.transform.Find("Fade").GetComponent<Image>();
        }
        else
        {
            _ui = existingMainUI;
        }

        // SoundManager 생성
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

        // Player 생성
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        if (nowPlayer == null)
        {
            player.TryGetComponent(out nowPlayer);
        }

        // 커서 세팅
        CursorManager cursorManager = GetComponent<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.Init_Cursor();
        }

        // 플레이어 데이터 로드
        DataManager.instance.LoadGame();
        // 이동된 맵에서 다시한번 저장
        DataManager.instance.SaveGame();
        // 페이드 인
        StartCoroutine(nameof(FadeIn));

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator FadeIn()
    {
        for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime)
        {
            Color newColor = fadeImage.color;
            newColor.a = alpha;
            fadeImage.color = newColor;
            yield return null;
        }
        // 다른 UI 가리지 않도록 비활성화
        fadeImage.gameObject.SetActive(false);
    }
    private IEnumerator InputUpArrow_co()
    {
        while (true)
        {
            // 위쪽 방향키 누를때마다 포탈에서 눌렀는지 확인
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
