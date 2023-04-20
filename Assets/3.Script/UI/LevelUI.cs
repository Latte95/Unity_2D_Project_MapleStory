using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    Image image;
    [SerializeField]
    int level;

    private void Start()
    {
        TryGetComponent(out image);
    }

    private void Update()
    {
        level = GameManager.Instance.nowPlayer.Level;
        switch (gameObject.name)
        {
            case "Level1":
                image.sprite = Resources.Load<Sprite>("Level/" + level%10);
                break;
            case "Level10":
                image.sprite = Resources.Load<Sprite>("Level/" + (int)(level*0.1));
                break;
        }
    }
}
