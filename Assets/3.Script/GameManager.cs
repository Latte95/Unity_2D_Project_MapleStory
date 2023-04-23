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
        // ���̺�
        if (Input.GetKeyDown(KeyCode.F11))
        {
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlaySfx(Define.Sfx.Transform);
            player.transform.position += player.transform.localScale.x * 3 * Vector3.left;
        }

        // ��� ȹ��
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
        //string currentSceneName = SceneManager.GetActiveScene().name;
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

    public void LoadSceneAndData(Define.Scene targetScene)
    {
        if (portal != null)
        {
            player.transform.position = portal.position;
        }
        // �÷��̾� ������ ����
        DataManager.instance.SaveGame();

        // �� �ε尡 �Ϸ�Ǹ� ����� �޼��带 ����
        SceneManager.sceneLoaded += OnSceneLoaded;

        // �� �ε�
        SceneManager.LoadScene(targetScene.ToString());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Canvas �Ҵ�
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
        // soundManager �Ҵ�
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

        // Player ������Ʈ �Ҵ�
        if (player == null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            player = playerInstance;
        }
        if (nowPlayer == null)
        {
            player.TryGetComponent(out nowPlayer);
        }

        // CursorManager�� transform_cursor �Ҵ�
        CursorManager cursorManager = GetComponent<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.Init_Cursor();
        }

        // Main Camera ����
        CameraControl cameraControl = FindObjectOfType<CameraControl>();

        // �÷��̾� ������ �ε�
        DataManager.instance.LoadGame();

        DataManager.instance.SaveGame();
        // �̺�Ʈ ���� ����
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
