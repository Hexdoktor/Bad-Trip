using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{

    public int healthAmount = 25;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            playerHealth.currentHealth += healthAmount;
            
            FindObjectOfType<AudioManager>().Play("NomNom");

            playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth, 0, playerHealth.maxHealth);
            
            playerHealth.healthBar.SetHealth(playerHealth.currentHealth);

            Destroy(gameObject);
        }
    }




}
