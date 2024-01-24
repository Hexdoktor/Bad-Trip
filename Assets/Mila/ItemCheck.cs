using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCheck : MonoBehaviour
{
    public List<string> items = new List<string>();
    BoxCollider2D collider;
    [SerializeField] GameObject speechBubble;
    [SerializeField] TMP_Text speechBubbleText;
    [SerializeField] GameObject player;
    [SerializeField] PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
       collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            if (items.Count > 0)
            {
                collider.enabled = true;
                speechBubble.SetActive(true);
                StartCoroutine(Bubble(3));
                playerMovement.horizontalMove = -1f;
                string itemsText = string.Join(", ", items);

           
                speechBubbleText.text = "Heck, I'm still missing my " + itemsText + ".";
            }
            else
            {
                collider.enabled = false;
            }

        }
    }
    Vector3 offset = new Vector3(5, 6, 0);
    IEnumerator Bubble(float time)
    {
        speechBubble.SetActive(true);
     
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            speechBubble.transform.position = player.transform.position + offset;
            playerMovement.horizontalMove = -40;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerMovement.horizontalMove = 0;
        speechBubble.SetActive(false);
    }

}
