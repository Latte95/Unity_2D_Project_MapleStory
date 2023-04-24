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
        // ���̺�
        if (Input.GetKeyDown(KeyCode.F11))
        {
            // ������ ����
            FindObjectOfType<QuickSlotManager>().SaveData(ref nowPlayer);
            _ui.EquipUI.SetActive(true);
            FindObjectOfType<EquipUI>().SaveData(ref nowPlayer);
            _ui.EquipUI.SetActive(_ui.activeEquip);
            DataManager.instance.SaveGame();
        }
        // ������ �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.F12))
        {
            nowPlayer.Init(4, 4, 4, 4);
            DataManager.instance.SaveGame();
        }
        // ��Ȱ�� �׽�Ʈ ���� �ڷ���Ʈ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlaySfx(Define.Sfx.Transform);
            player.transform.position += player.transform.localScale.x * 3 * Vector3.left;
        }
        // ��Ȱ�� �׽�Ʈ ���� ��� ȹ��
        if (Input.GetKeyDown(KeyCode.F6))
        {
            nowPlayer.inventory.GetItem("��");
            nowPlayer.inventory.GetItem(01040002, 2);
            nowPlayer.inventory.GetItem(1060002);
        }
    }

    // �÷��̾� ��ġ ���� �� �ε�
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

    // Ŭ�� �̺�Ʈ
    public void OnSaveGameClicked()
    {
        DataManager.instance.SaveGame();
    }
    public void GameExit()
    {
        DataManager.instance.SaveGame();
        Application.Quit();
    }

    // ���� �� ����
    public void FadeOutAndLoadScene(Define.Scene targetScene)
    {
        GameObject canvasInstance = Instantiate(canvasPrefab);
        canvasInstance.name = canvasPrefab.name;

        _ui = canvasInstance.GetComponent<MainUI>();
        fadeImage = canvasInstance.transform.Find("Fade").GetComponent<Image>();
        fadeImage.gameObject.SetActive(true);

        // ���̵�ƿ�
        Color newColor = fadeImage.color;
        newColor.a = 1;
        fadeImage.color = newColor;

        // Load scene
        LoadSceneAndData(targetScene);
    }
    // ���� �� �ε�
    public void LoadSceneAndData(Define.Scene targetScene)
    {
        if (portal != null)
        {
            player.transform.position = portal.position;
        }
        // �÷��̾� ������ ����
        DataManager.instance.SaveGame();

        // �� �ε尡 �Ϸ�Ǹ� ����
        SceneManager.sceneLoaded += OnSceneLoaded;
        // �� �ε�
        SceneManager.LoadScene(targetScene.ToString());
    }
    // �� �ε� ���� ��
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Canvas ����
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

        // SoundManager ����
        if (soundManager == null)
        {
            GameObject soundInstance = Instantiate(soundPrefab);
            soundManager = FindObjectOfType<SoundManager>();
        }

        // StageData �Ҵ�
        if (stageData == null)
        {
            stageData = Resources.Load<StageData>("StageData/" + nowPlayer.Scene.ToString());
        }

        // Player ����
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        if (nowPlayer == null)
        {
            player.TryGetComponent(out nowPlayer);
        }

        // Ŀ�� ����
        CursorManager cursorManager = GetComponent<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.Init_Cursor();
        }

        // �÷��̾� ������ �ε�
        DataManager.instance.LoadGame();
        // �̵��� �ʿ��� �ٽ��ѹ� ����
        DataManager.instance.SaveGame();
        // ���̵� ��
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
        // �ٸ� UI ������ �ʵ��� ��Ȱ��ȭ
        fadeImage.gameObject.SetActive(false);
    }
    private IEnumerator InputUpArrow_co()
    {
        while (true)
        {
            // ���� ����Ű ���������� ��Ż���� �������� Ȯ��
            yield return inputUpArrow_wait;
            int layerMask = LayerMask.GetMask("Portal");
            Collider2D col = Physics2D.OverlapCircle(player.transform.position, 0.5f, layerMask);

            // Portal �����ϸ� ���ε�
            if (col != null)
            {
                soundManager.PlaySfx(Define.Sfx.Portal);
                col.TryGetComponent(out portal);
                FadeOutAndLoadScene(portal.scene);
            }
        }
    }
}
