using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour, ICollectible
{

    public static event Action OnItemCollected;
   
        
    public void Collect()
    {
        Debug.Log("Collected Item");
        FindObjectOfType<AudioManager>().Play("ItemCollect");
        Destroy(gameObject);
        OnItemCollected?.Invoke();
    }
}
