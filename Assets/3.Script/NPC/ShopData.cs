using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;
    public List<int> itemID;


    void Start()
    {
        TryGetComponent(out sprite);
    }
}
