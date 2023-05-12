using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrawler : MonoBehaviour
{
    public Animator animator;
    public GameObject bloodsplat;
    public GameObject missiletype;

    public float moveSpeed;
    public float moveHeight; // up from current position
    public bool waitforplayer; // doesn't move until player is close enough, is set to false when player got close enough
    public float actdistance; // activation distance
    public float attackdistance; // attack distance
    public int maxHealth = 70;
    int currentHealth;

    public int damage;

    float attack_time; // doing an attack
    bool attack_hit; // marker for the current attack hit player, cleared when attack has finished
    float pain_time; // being hurt
    float move_time; // 
    float move_dir; // -1 = down, 0 = not moving or randomize, 1 = up
    float spawnheight;

    private Rigidbody2D rb; // this one
    private Rigidbody2D target; // target this player
    private Vector2 size;
    private Collider2D hitbox;
    private RaycastHit2D hit;

    public LayerMask Ground;
    public LayerMask Player;


    void ChangeMoveDir()
    {
        // randomize at level startup
        if(move_dir == 0) {
            if(Random.value < 0.5f)
                move_dir = 1;
            else
                move_dir = -1;
        }
        // otherwise flip it
        else
            move_dir *= -1;
    }

    void CheckMoveDir()
    {
        if(rb.position.y >= spawnheight + moveHeight)
            ChangeMoveDir();
        else if(rb.position.y <= spawnheight)
            ChangeMoveDir();
    }

    void MoveEnemy()
    {
//        animator.SetBool("Walk", true);
//        transform.localScale = new Vector3(1, -1*move_dir, 1);
        transform.Translate(0, move_dir * moveSpeed * Time.deltaTime, 0);
    }

    void EnemySpitAttack()
    {
        Vector3 dir;

        Collider2D col;
        col = Physics2D.OverlapCircle(rb.position, attackdistance, Player);
        if(col)
            target = col.attachedRigidbody;
        GameObject missile = Instantiate(missiletype, rb.position, Quaternion.identity);
        // shoot toward the target
        dir = rb.position - target.position;
        float angle = Vector3.Angle(dir, transform.right);
        missile.GetComponent<Missiles>().SetAngle(angle);
        
        attack_time = Time.timeSinceLevelLoad + 1.5f;
    }

    // try to find the player at the specified distance
    bool FindPlayer(float dist)
    {
        return (Physics2D.OverlapCircle(rb.position, dist, Player));
    }

    //take damage when hit by player
    public void TakeDamage(int damage)
    {
        ParticleSystem ps;
        currentHealth -= damage;

//    Debug.Log("EnemyCrawler got hit: " + damage + " health now: " + currentHealth);

//        animator.SetTrigger("Hurt");
//        animator.ResetTrigger("Attack");

        pain_time = Time.timeSinceLevelLoad + 1.0f;

        GameObject blood = Instantiate(bloodsplat, rb.position, Quaternion.identity);
        ps = blood.GetComponent<ParticleSystem>();
        // change to green blood
        var main = ps.main;
        main.startColor = new Color(0.5f, 1f, 0.25f, 1f);
        var color = ps.colorOverLifetime;
        color.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys( new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );
        color.color = grad;
        blood.GetComponent<ParticleSystem>().Play();
        
        if(currentHealth <= 0)
        {
            Die();
        }
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    //disable enemy when dead
    void Die()
    {
        Debug.Log("Enemy Died!");
//        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        size = hitbox.bounds.size;
        spawnheight = rb.position.y;
        // default values if nothing set in Unity
        if(moveSpeed == 0)
            moveSpeed = 5f;
        if(moveHeight == 0)
            moveHeight = 10f;
        if(actdistance < 25f)
            actdistance = 25f;
        attackdistance = 20f;
        ChangeMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        // the enemy is active
        if(waitforplayer == false) {
            CheckMoveDir();
/*
            // can't attack if feeling pain
            if(Time.timeSinceLevelLoad > pain_time) {
                if(Time.timeSinceLevelLoad < attack_time)
                    FacePlayer();
                if(Time.timeSinceLevelLoad > attack_time - 1.0f && Time.timeSinceLevelLoad < attack_time - 0.25f)
                    EnemyMeleeHit();
            }
            else
                animator.ResetTrigger("Hurt");
*/
            // move or attack
            if(Time.timeSinceLevelLoad > attack_time) {
                // check for attack
                attack_hit = false;
                if(FindPlayer(attackdistance) && Random.value < 0.05f && Time.timeSinceLevelLoad > attack_time + 0.25f + Random.value * 0.5f)
                    EnemySpitAttack();
                // otherwise move
                else {
                    MoveEnemy();
                    // this enemy can jump
                    //if(jumpForce > 0 && Random.value < 0.01f && IsGrounded() && Time.timeSinceLevelLoad > move_time + 0.5f)
                    //    JumpEnemy();
                }
            }
        }
        // wait for player to get close enough
        else {
//            animator.SetBool("Walk", false);
            if(FindPlayer(actdistance))
                waitforplayer = false;
        }
    }

    // bumped into something
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Untagged")
            return;
        // colliding with another enemy or a wall, turn away
        if(col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wall") {
        //    ChangeMoveDir();
            return;
        }
        transform.Translate(0, move_dir * - 1 * moveSpeed * Time.deltaTime, 0);
        col.gameObject.SendMessage("TakeDamage", 5);
        //ChangeMoveDir();
    }
}
