using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDeactivator : MonoBehaviour
{
    [SerializeField] GameObject effectsToActivate;
    [SerializeField] GameObject effectsToDeactivate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effectsToActivate.SetActive(true);
            effectsToDeactivate.SetActive(false);
        }
    }
}
