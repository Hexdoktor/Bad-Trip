﻿using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Animator animator;

    public GameObject bloodsplat;

    public float moveSpeed;
    public float jumpForce; // set to allow jumping
    public float stepheight;
    public float dropheight;
    public bool waitforplayer; // doesn't move until player is close enough (or moves, but doesn't step up / drop down), is set to false when player got close enough
    public float actdistance; // activation distance
    public float attackdistance; // attack distance

    bool landed; // maybe useless / didn't want to work right

    float attack_time; // doing an attack
    bool attack_hit; // marker for the current attack hit player, cleared when attack has finished
    float move_time; // used to be time when to turn around, but is now used for jumping time
    float move_dir; // -1 = left, 0 = not moving or randomize, 1 = right

    public Rigidbody2D rb;
    private Rigidbody2D target;
    public Vector2 size;
    private Collider2D hitbox;
    private RaycastHit2D hit;

    public Transform pos;
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
        // check for a dropoff down
        Vector2 vec = new Vector2(0, dropheight);
        hit = Physics2D.Linecast(rb.position, rb.position - vec, Ground);
        if(!hit) {
            if(Time.timeSinceLevelLoad > move_time)
                ChangeMoveDir();
            return;
        }
        // on a slope
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        // check for stepping up or a wall
        if(move_dir == 0)
            vec = new Vector2(-1.0f, 0.0f);
        else
            vec = new Vector2(1.0f, 0.0f);
        // hit something
        if(Physics2D.CapsuleCast(rb.position, size*0.95f, CapsuleDirection2D.Vertical, 0, vec, stepheight, Ground)) {
            // can be stepped up?
            if(!Physics2D.CapsuleCast(rb.position + new Vector2(0, stepheight + 0.1f), size*0.95f, CapsuleDirection2D.Vertical, 0, vec, stepheight, Ground)) {
                if(slopeAngle == 0)
                    transform.Translate(0, stepheight + 0.1f, 0);
            }
            else
                ChangeMoveDir();
        }
    }

    void MoveEnemy()
    {
        animator.SetBool("Walk", true);
        transform.localScale = new Vector3(-1*move_dir, 1, 1);
        transform.Translate(move_dir * moveSpeed * Time.deltaTime, 0, 0);
    }

    bool IsGrounded()
    {
        Vector2 vec = new Vector2(0, 1.5f);
        if (Physics2D.Linecast(rb.position, rb.position - vec, Ground))
            return true;
        //else if (Physics2D.Linecast(rb.position, rb.position - vec, enemyLayer))
        //    return true;
        else
            return false;
    }

    void JumpEnemy()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        move_time = Time.timeSinceLevelLoad + 2.5f;
        landed = false;
    }

    // turns to face & charge towards player
    void FacePlayer()
    {
        if(target) {
            if(rb.position.x < target.position.x)
                move_dir = 1;
            else
                move_dir = -1;
            transform.localScale = new Vector3(-1*move_dir, 1, 1);
            transform.Translate(move_dir * moveSpeed*1.5f * Time.deltaTime, 0, 0);
        }
    }
    
    // call this function from specific frame or frames, one attack should only hit once even if it could hit on multiple frames
    void EnemyMeleeHit()
    {
        // has already hit
        if(attack_hit == true)
            return;
        // attack direction determined by the move_dir
        Vector2 vec = new Vector2(move_dir * 5.0f, 0);
        hit = Physics2D.Linecast(rb.position, rb.position + vec, Player);
        if(hit) {
            //col.gameObject.SendMessage("ApplyDamage", 1); // sends a message to the player, calling this function named ApplyDamage with value 1
            hit.collider.attachedRigidbody.AddForce(Vector2.right * 5, ForceMode2D.Impulse); // maybe the damage function pushes the player back?
            rb.AddForce(Vector2.right * 5 * move_dir, ForceMode2D.Impulse);
            // for now here's just a debug log about the hit connecting
            Debug.Log("Enemy hit Player, ouch!");
            
            GameObject blood = Instantiate(bloodsplat, hit.collider.attachedRigidbody.position, Quaternion.identity);
            blood.GetComponent<ParticleSystem>().Play();
            
            attack_hit = true;
        }
    }
    
    // starts the attack
    void EnemyMeleeAttack()
    {
        animator.SetTrigger("Attack");
        Collider2D col;
        col = Physics2D.OverlapCircle(rb.position, attackdistance, Player);
        if(col)
            target = col.attachedRigidbody;
        attack_time = Time.timeSinceLevelLoad + 1.5f;
    }
    
    // try to find the player at the specified distance
    bool FindPlayer(float dist)
    {
        return (Physics2D.OverlapCircle(rb.position, dist, Player));
    }


    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        size = hitbox.bounds.size;
        // default values if nothing set in Unity
        if(moveSpeed == 0)
            moveSpeed = 5;
        if(stepheight == 0)
            stepheight = 1.5f;
        if(dropheight == 0)
            dropheight = 4.0f;
        if(actdistance < 15f)
            actdistance = 15f;
        attackdistance = 7.5f;
        ChangeMoveDir();
        landed = true;
    }


    void Update()
    {
        // the enemy is active
        if(waitforplayer == false) {
            CheckMoveDir();
            if(Time.timeSinceLevelLoad < attack_time)
                FacePlayer();
            if(Time.timeSinceLevelLoad > attack_time - 1.25f && Time.timeSinceLevelLoad < attack_time - 0.5f)
                EnemyMeleeHit();
            if(Time.timeSinceLevelLoad > attack_time) {
                // check for attack
                attack_hit = false;
                if(FindPlayer(attackdistance) && Random.value < 0.01f && Time.timeSinceLevelLoad > attack_time + 0.5f + Random.value * 0.5f)
                    EnemyMeleeAttack();
                // otherwise move
                else {
                    MoveEnemy();
                    // this enemy can jump
                    if(jumpForce > 0 && Random.value < 0.01f && IsGrounded() && Time.timeSinceLevelLoad > move_time + 0.5f)
                        JumpEnemy();
                }
            }
        }
        // wait for player to get close enough
        else {
            animator.SetBool("Walk", false);
            if(FindPlayer(actdistance))
                waitforplayer = false;
        }
    }


    // hits an invisible trigger, not currently in use, would need to check for specific trigger?
    void OnTriggerEnter2D_notused(Collider2D col)
    {
        ChangeMoveDir();
    }

    // bumped into something
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Untagged")
            return;
        // colliding with another enemy or a wall, turn away
        if(col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wall") {
            ChangeMoveDir();
            return;
        }
        transform.Translate(move_dir * - 1 * moveSpeed * Time.deltaTime, 0, 0);
        //col.gameObject.SendMessage("ApplyDamage", 1);
        ChangeMoveDir();
    }
}