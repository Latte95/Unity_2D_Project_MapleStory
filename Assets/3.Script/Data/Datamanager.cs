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
    // Singleton
    public static DataManager instance;

    public Player nowPlayer;
    private List<IDataPersistence> dataPersistenceObjects;

    private string path;
    private string fileName;

    private static int keySizes = 32;
    private byte[] key;
    private static readonly byte[] salt = new byte[] { 0x26, 0x19, 0x36, 0x29, 0x3F, 0x10, 0x01, 0x1A };
    private const int Iterations = 10000;

    // æ∆¿Ã≈€
    //public string[] var_name;
    //public float[] var;
    //public string[] switch_name;
    //public bool[] switches;
    //public List<Item> itemList = new List<Item>();
    public ItemDataBase itemDataBase;

    // Initialize
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
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private void OnEnable()
    {
        itemDataBase = new ItemDataBase();
        nowPlayer = FindObjectOfType<Player>();
    }

    public void LoadGame()
    {
        if(nowPlayer == null)
        {
            nowPlayer = FindObjectOfType<Player>();
        }
        fileName = "saveData.json";

        // Generate a key based on the file name and salt
        using (var pbkdf2 = new Rfc2898DeriveBytes(fileName, salt, Iterations))
        {
            key = pbkdf2.GetBytes(keySizes);
        }

        // Load player data from JSON
        PlayerData playerData = Load();
        if (playerData != null)
        {
            nowPlayer.SetData(playerData);
        }
        // Update all data persistence objects
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(nowPlayer);
        }
    }

    // Save data to save file and update all data persistence objects
    public void SaveGame()
    {
        // Update all data persistence objects
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref nowPlayer);
        }
        // Save player data as JSON
        Save(nowPlayer.ToPlayerData());
    }

    // Reads data stored in a json file
    public PlayerData Load()
    {
        string fullPath = Path.Combine(path, fileName);
        //Player loadedData = null;
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
                        //string decryptedData = Decrypt(encryptedData);
                        loadedData = JsonUtility.FromJson<PlayerData>(encryptedData);
                        //loadedData = JsonUtility.FromJson<Player>(encryptedData);
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

    // Save data as a json file
    public void Save(PlayerData data)
    {
        string fullPath = Path.Combine(path, fileName);
        try
        {
            // Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize player information in JSON and save to a file
            string dataToStore = JsonUtility.ToJson(data, true);
            string encryptedData = Encrypt(dataToStore);
            using (StreamWriter writer = new StreamWriter(fullPath, false))
            {
                writer.Write(dataToStore);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save data: " + fullPath + "\n" + e.Message);
        }
    }

    // Encrypts the given string using AES encryption
    // and returns the result as a Base64-encoded string.
    private string Encrypt(string data)
    {
        // Convert the input string to a byte array
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

        // Create a new instance of the AES encryption algorithm
        using (Aes aes = Aes.Create())
        {
            // Generate a key and initialization vector (IV)
            // using a PBKDF2 key derivation function with the given key, salt, and iterations
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations))
            {
                // Set the key and IV properties of the AES algorithm
                aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
                aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);
            }

            // Encrypt the input byte array using the AES algorithm
            // and write the result to a memory stream
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                // Convert the encrypted byte array to a Base64-encoded string and return it
                byte[] cipherTextBytes = ms.ToArray();
                return Convert.ToBase64String(cipherTextBytes);
            }
        }
    }

    // Decrypts the given Base64-encoded string using AES encryption
    // and returns the result as a plain text string
    private string Decrypt(string encryptedData)
    {
        // Convert the input string from Base64 to a byte array
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedData);

        // Create a new instance of the AES encryption algorithm
        using (Aes aes = Aes.Create())
        {
            // Generate a key and initialization vector (IV)
            // using a PBKDF2 key derivation function with the given key, salt, and iterations
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations))
            {
                // Set the key and IV properties of the AES algorithm
                aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
                aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);
            }

            // Decrypt the input byte array using the AES algorithm
            // and write the result to a memory stream
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                // Convert the decrypted byte array to a plain text string and return it
                byte[] plainTextBytes = ms.ToArray();
                return Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
            }
        }
    }


    // Sava data file initialize
    public void ClearData()
    {
        //nowPlayer = new Player();
        Save(nowPlayer.ToPlayerData());
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>()
            .ToList();
    }
}