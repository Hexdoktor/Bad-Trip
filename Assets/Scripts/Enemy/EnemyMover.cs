using UnityEngine;

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
    public int maxHealth = 100;
    int currentHealth;

    public int damage;
    public PlayerHealth playerHealth;

    bool landed; // maybe useless / didn't want to work right

    float attack_time; // doing an attack
    float attack_count;
    bool attack_hit; // marker for the current attack hit player, cleared when attack has finished
    float pain_time; // being hurt
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
        if(Time.timeSinceLevelLoad < move_time)
            return;
        move_time = Time.timeSinceLevelLoad + 0.1f;
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
            if(landed)
                ChangeMoveDir();
            landed = false;
            return;
        }
        else {
            vec = new Vector2(0, size.y * 0.5f + 0.1f);
            if(Physics2D.Linecast(rb.position, rb.position - vec, Ground)) {
                // just landed, add some move_time because apparently landing activates the stepping up stairs code, huh
                if(!landed)
                    move_time = Time.timeSinceLevelLoad + 0.25f;
                landed = true;
            }
            else
                landed = false;
        }
        // on a slope
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        // check for stepping up or a wall
        if(move_dir == 0)
            vec = new Vector2(-1.0f, 0.0f);
        else
            vec = new Vector2(1.0f, 0.0f);
        // hit something
        if(Physics2D.CapsuleCast(rb.position, size*0.99f, CapsuleDirection2D.Vertical, 0, vec, size.x+0.1f, Ground)) {
            // can be stepped up?
            if(!Physics2D.CapsuleCast(rb.position + new Vector2(0, stepheight + 0.1f), size*0.99f, CapsuleDirection2D.Vertical, 0, vec, size.x+0.1f, Ground)) {
                if(slopeAngle == 0 && landed && Time.timeSinceLevelLoad > move_time) {
                    transform.Translate(0, stepheight + 0.1f, 0);
                    move_time = Time.timeSinceLevelLoad + 0.1f;
                }
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
        Vector2 vec = new Vector2(0, size.y * 0.5f + 0.5f);
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
        move_time = Time.timeSinceLevelLoad + 0.5f;
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
            if(Time.timeSinceLevelLoad > move_time) {
                transform.localScale = new Vector3(-1*move_dir, 1, 1);
                move_time = Time.timeSinceLevelLoad + 0.5f;
            }
            if(landed == true)
                transform.Translate(move_dir * moveSpeed*1.5f * Time.deltaTime, 0, 0);
        }
    }

    // checks if the boss earthquake attack can hit player
    void BossEarthQuakeHitPlayer()
    {
        // has already hit
        if(attack_hit == true)
            return;
        Collider2D col;
        col = Physics2D.OverlapBox(rb.position + new Vector2(0, -3.5f), new Vector2(20, 4), 0, Player);
        if(col) {
            col.attachedRigidbody.AddForce(new Vector2(-15 + Random.value * 30, 30), ForceMode2D.Impulse);

            // do the damage
            col.gameObject.SendMessage("TakeDamage", damage);
            
            // spawn blood
            GameObject blood = Instantiate(bloodsplat, col.attachedRigidbody.position, Quaternion.identity);
            blood.GetComponent<ParticleSystem>().Play();

            attack_hit = true;
        }
        move_time = Time.timeSinceLevelLoad + 0.5f;
        attack_time = Time.timeSinceLevelLoad + 1.5f;
    }

    // checks if the boss has landed to do the earthquake attack
    void BossEarthQuakeHitGround()
    {
        attack_count++;
        if(attack_count == 15)
            rb.AddForce(Vector2.up * jumpForce * -1.0f, ForceMode2D.Impulse);
        // not on ground yet
        if(!IsGrounded() || attack_count < 4)
            return;
        animator.SetTrigger("AttackQuakeHit");
        landed = true;
        attack_time = Time.timeSinceLevelLoad + 1.5f;
    }

    // starts the boss earthquake attack
    void BossEarthQuakeAttack()
    {
        Debug.Log("Jump one");
        animator.SetTrigger("AttackQuake");
        Collider2D col;
        col = Physics2D.OverlapCircle(rb.position, attackdistance, Player);
        if(col)
            target = col.attachedRigidbody;
        rb.AddForce(Vector2.up * jumpForce * 1.5f, ForceMode2D.Impulse);
        attack_count = 0;
        move_time = Time.timeSinceLevelLoad + 0.5f;
        landed = false;
        attack_time = Time.timeSinceLevelLoad + 1.5f;
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
            // maybe the damage function pushes the player back? Does it happen?
            hit.collider.attachedRigidbody.AddForce(Vector2.right * 5, ForceMode2D.Impulse);
            rb.AddForce(Vector2.right * 5 * move_dir, ForceMode2D.Impulse);

            // do the damage
            hit.collider.gameObject.SendMessage("TakeDamage", damage);
            
            // spawn blood
            GameObject blood = Instantiate(bloodsplat, hit.collider.attachedRigidbody.position, Quaternion.identity);
            blood.GetComponent<ParticleSystem>().Play();
            
            attack_hit = true;
        }
    }
    
    // starts the attack
    void EnemyMeleeAttack()
    {
        Debug.Log("normal hit");
        animator.SetTrigger("Attack");
        Collider2D col;
        col = Physics2D.OverlapCircle(rb.position, attackdistance, Player);
        if(col)
            target = col.attachedRigidbody;
        attack_time = Time.timeSinceLevelLoad + 1.5f;
    }

    void EnemyBodyslamAttack()
    {
        Debug.Log("bodyslam");
        animator.SetTrigger("Attack2");
        Collider2D col;
        col = Physics2D.OverlapCircle(rb.position, attackdistance, Player);
        if (col)
            target = col.attachedRigidbody;
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

        FindObjectOfType<AudioManager>().Play("StickSwat");

        animator.SetTrigger("Hurt");
        animator.ResetTrigger("Attack");


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
        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }




    void Start()
    {
        currentHealth = maxHealth;
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
        if(attackdistance < 6.5f)
            attackdistance = 6.5f;
        ChangeMoveDir();
        landed = true;
    }

    //int randomIndex;
    void Update()
    {
        // the enemy is active
        if(waitforplayer == false) {
            CheckMoveDir();
            // can't attack if feeling pain
            if(Time.timeSinceLevelLoad > pain_time) {
                if(Time.timeSinceLevelLoad < attack_time)
                    FacePlayer();
            }
            else
                animator.ResetTrigger("Hurt");
            // move or attack
            if(Time.timeSinceLevelLoad > attack_time) {
                // check for attack
                attack_hit = false;
                if(FindPlayer(attackdistance) && Random.value < 0.05f && Time.timeSinceLevelLoad > attack_time + 0.25f + Random.value * 0.5f) {
                    if (rb.name == "BossKarhu" && Random.value < 0.85f)
                 
                    BossEarthQuakeAttack();
                    else
                    //    randomIndex = Random.Range(0, 2);
                    //    if (randomIndex == 1)
                    //{
                        EnemyMeleeAttack();
                    //}
                    //    else
                    //{
                    //    EnemyBodyslamAttack();
                    //}
                     
                }
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
            move_time = 0;
            ChangeMoveDir();
            return;
        }
        transform.Translate(move_dir * - 1 * moveSpeed * Time.deltaTime, 0, 0);
        ChangeMoveDir();
    }
}
