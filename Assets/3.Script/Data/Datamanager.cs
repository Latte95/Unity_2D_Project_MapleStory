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

    // 아이템
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

        // 키 생성
        using (var pbkdf2 = new Rfc2898DeriveBytes(fileName, salt, Iterations))
        {
            key = pbkdf2.GetBytes(keySizes);
        }

        // Json 읽기
        PlayerData playerData = Load();
        // 읽은 데이터 플레이어에 덮어쓰기
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
                        // 복호화
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
        // 저장해야 될 데이터들 Player데이터에 대입
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref nowPlayer);
        }
        // Json 데이터로 저장
        Save(nowPlayer.ToPlayerData());
    }  
    public void Save(PlayerData data)
    {
        string fullPath = Path.Combine(path, fileName);
        try
        {
            // 저장 경로 없으면 새로 만듦
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);
            // 데이터 암호화
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
        // 데이터(문자열)를 Byte배열로 변환
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

        using (Aes aes = Aes.Create())
        {
            // 키와 IV 생성
            // PBKDF2 키 파생 함수 이용
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations))
            {
                // bit -> Byte
                aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
                aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);
            }

            // 암호화
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                // Byte배열을 다시 문자열로 변환
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