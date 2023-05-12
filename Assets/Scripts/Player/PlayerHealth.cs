using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public HealthBar healthBar;

    private bool isDead;

    public GameManager gameManager;
    
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

   
    void Update()
    {

    }

    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        FindObjectOfType<AudioManager>().Play("TakingDamage");
        
        if (currentHealth <= 0 && !isDead)

        {
            isDead = true;

            FindObjectOfType<AudioManager>().Play("PlayerDeath");

            gameObject.SetActive(false);
            gameManager.gameOver();
        }
    }
}
