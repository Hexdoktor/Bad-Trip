using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item : MonoBehaviour, ICollectible
{
    [SerializeField] GameObject collectibleUI;
    [SerializeField] Sprite newSprite;

    public static event Action OnItemCollected;


    public void Collect()
    {
        Debug.Log("Collected Item");
        collectibleUI.GetComponent<Image>().sprite = newSprite;

        FindObjectOfType<AudioManager>().Play("ItemCollect");
        Destroy(gameObject);
        OnItemCollected?.Invoke();

    }
}

