using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public Player nowPlayer;
    private List<IDataPersistence> dataPersistenceObjects;

    private string path;
    private string fileName;

    private static int keySizes = 32;
    private byte[] key;
    private static readonly byte[] salt = new byte[] { 0x26, 0x19, 0x36, 0x29, 0x3F, 0x10, 0x01, 0x1A };
    private const int Iterations = 10000;

    // ������
    public ItemDataBase itemDataBase;

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        #endregion
        path = Path.Combine(Application.dataPath, "Saves/");
    }

    private void OnEnable()
    {
        itemDataBase = new ItemDataBase();
        nowPlayer = FindObjectOfType<Player>();
    }

    private void Start()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void LoadGame()
    {
        if (nowPlayer == null)
        {
            nowPlayer = FindObjectOfType<Player>();
        }
        fileName = "saveData.json";

        // Ű ����
        using (var pbkdf2 = new Rfc2898DeriveBytes(fileName, salt, Iterations))
        {
            key = pbkdf2.GetBytes(keySizes);
        }

        // Json �б�
        PlayerData playerData = Load();
        // ���� ������ �÷��̾ �����
        if (playerData != null)
        {
            nowPlayer.SetData(playerData);
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(nowPlayer);
        }
    }
    public PlayerData Load()
    {
        string fullPath = Path.Combine(path, fileName);
        PlayerData loadedData = null;
        try
        {
            if (File.Exists(fullPath))
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string encryptedData = reader.ReadToEnd();
                        // ��ȣȭ
                        string decryptedData = Decrypt(encryptedData);
                        loadedData = JsonUtility.FromJson<PlayerData>(decryptedData);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data: " + fullPath + "\n" + e.Message);
        }
        return loadedData;
    }


    public void SaveGame()
    {
        // �����ؾ� �� �����͵� Player�����Ϳ� ����
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref nowPlayer);
        }
        // Json �����ͷ� ����
        Save(nowPlayer.ToPlayerData());
    }  
    public void Save(PlayerData data)
    {
        string fullPath = Path.Combine(path, fileName);
        try
        {
            // ���� ��� ������ ���� ����
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);
            // ������ ��ȣȭ
            string encryptedData = Encrypt(dataToStore);
            using (StreamWriter writer = new StreamWriter(fullPath, false))
            {
                writer.Write(encryptedData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save data: " + fullPath + "\n" + e.Message);
        }
    }

    // AES
    private string Encrypt(string data)
    {
        // ������(���ڿ�)�� Byte�迭�� ��ȯ
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

        using (Aes aes = Aes.Create())
        {
            // Ű�� IV ����
            // PBKDF2 Ű �Ļ� �Լ� �̿�
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations))
            {
                // bit -> Byte
                aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
                aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);
            }

            // ��ȣȭ
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                // Byte�迭�� �ٽ� ���ڿ��� ��ȯ
                byte[] cipherTextBytes = ms.ToArray();
                return Convert.ToBase64String(cipherTextBytes);
            }
        }
    }
    private string Decrypt(string encryptedData)
    {
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedData);

        using (Aes aes = Aes.Create())
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations))
            {
                aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
                aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                byte[] plainTextBytes = ms.ToArray();
                return Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
            }
        }
    }

    public void ClearData()
    {
        nowPlayer.Init(4,4,4,4);
        Save(nowPlayer.ToPlayerData());
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>()
            .ToList();
    }
}