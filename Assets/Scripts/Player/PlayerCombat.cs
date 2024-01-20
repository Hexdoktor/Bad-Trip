using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;
    
    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    [SerializeField] PlayerMovement playerMovementScript;
    
    
    
    // Update is called once per frame
    void Update()
    {
        //if (Time.time >= nextAttackTime)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftShift))
        //    {
        //        Attack();
        //        nextAttackTime = Time.time + 1f / attackRate;         
        //    }
        //}
        

    }
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        if (playerMovementScript.crouch)
        {
            animator.SetTrigger("CrouchHit");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
       
        FindObjectOfType<AudioManager>().Play("StickSwing");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            //enemy.GetComponent<EnemyMover>().TakeDamage(attackDamage);
            enemy.gameObject.SendMessage("TakeDamage", attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
