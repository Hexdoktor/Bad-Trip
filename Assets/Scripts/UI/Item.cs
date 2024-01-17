using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Item : MonoBehaviour, ICollectible
{
    [SerializeField] GameObject collectibleUI;
    [SerializeField] Sprite newSprite;
    [SerializeField] ItemCheck itemCheckScript;

    public static event Action OnItemCollected;


    public void Collect()
    {
        Debug.Log("Collected Item");
        collectibleUI.GetComponent<Image>().sprite = newSprite;
      
        itemCheckScript.items.RemoveAll(item => item == gameObject.name);

        FindObjectOfType<AudioManager>().Play("ItemCollect");
        Destroy(gameObject);
        OnItemCollected?.Invoke();

    }
}

