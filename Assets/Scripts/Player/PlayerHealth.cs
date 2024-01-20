using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [SerializeField] Animator animator;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerMovement playerMovementScript;
    [SerializeField] CircleCollider2D circleCollider;

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

            StartCoroutine(EndGame());
      
        }
    }

    IEnumerator EndGame()
    {
        isDead = true;
        playerInput.enabled = false;
        playerMovementScript.enabled = false;
        circleCollider.sharedMaterial = null;
        animator.SetTrigger("Death");
        FindObjectOfType<AudioManager>().Play("PlayerDeath");

        yield return new WaitForSeconds(3);

     
        gameObject.SetActive(false);
        animator.enabled = false;
        gameManager.gameOver();
    }

       
}
