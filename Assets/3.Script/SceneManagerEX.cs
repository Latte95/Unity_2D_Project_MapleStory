using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagerEX
{
    public static string loadScene;


    public static void LoadScene(Define.Scene type)
    {
        loadScene = System.Enum.GetName(typeof(Define.Scene), type);
        SceneManager.LoadScene(loadScene);
    }
}
