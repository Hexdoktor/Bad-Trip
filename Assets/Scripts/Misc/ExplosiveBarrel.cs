using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public Animator animator;
    public GameObject sparkpuff;
    public GameObject explosion;
    public int maxHealth = 20;
    int currentHealth;

    public LayerMask Damageables;

    private Rigidbody2D rb; // this one

    //take damage when hit by player
    public void TakeDamage(int damage)
    {
        if(currentHealth <= 0)
            return;

        ParticleSystem ps;
        currentHealth -= damage;

        GameObject sparks = Instantiate(sparkpuff, rb.position, Quaternion.identity);
        ps = sparks.GetComponent<ParticleSystem>();
        
        if(currentHealth <= 0)
            animator.SetTrigger("Burning");
    }

    // spawns sparks
    public void Sparks()
    {
        animator.ResetTrigger("Burning");
        ParticleSystem ps;
        GameObject sparks = Instantiate(sparkpuff, rb.position + new Vector2(0, 0.5f), Quaternion.identity);
        ps = sparks.GetComponent<ParticleSystem>();
    }

    // explosion
    void Die()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(rb.position, 10f, Damageables);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            float dmg;
            dmg = Vector2.Distance(rb.position, enemy.transform.position);
            dmg = 10f - dmg;
            dmg *= 12;

            enemy.gameObject.SendMessage("TakeDamage", (int)(dmg));
        }
        ParticleSystem ps;
        GameObject exp = Instantiate(explosion, rb.position, Quaternion.identity);
        ps = exp.GetComponent<ParticleSystem>();
        Destroy(gameObject, 0.5f);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
