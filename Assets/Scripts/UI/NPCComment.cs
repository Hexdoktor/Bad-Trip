using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCComment : MonoBehaviour
{

    public GameObject chatBubblePrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("Mumbling");

            GameObject chatBubble = Instantiate(chatBubblePrefab, transform.position + Vector3.up, Quaternion.identity);
            chatBubble.transform.SetParent(transform);
            Destroy(chatBubble, 6f);
        }
    }

}
