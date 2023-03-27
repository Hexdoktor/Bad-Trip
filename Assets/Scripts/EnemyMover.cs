using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    float sidemove = 0.0f;

    public float moveSpeed;
    public float jumpForce;
    public float stepheight;
    public float dropheight;
    public bool waitforplayer; // doesn't move until player is close enough (or moves, but doesn't step up / drop down), is set to false when player got close enough
    public float actdistance; // activation distance
    public bool allowjump; // the enemy can jump randomly, though suppose just setting the jumpForce to non zero would allow jumping

    bool landed;

    float move_time;
    float move_dir; // 0 = right, 1 = left, 2 = vertical (vertical may be a test/placeholder for attack)

    public Rigidbody2D rb;
    public Vector2 size;
    private Collider2D hitbox;
    private RaycastHit2D hit;

    public Transform pos;
    public LayerMask Ground;
    public LayerMask Player;


    void ChangeMoveDir()
    {
        if(move_dir == 0)
            move_dir = 1;
        else
            move_dir = 0;
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
        // left
        if(move_dir == 0) {
            sidemove = -moveSpeed * Time.deltaTime;
            //transform.localScale = new Vector3(-0.35f, 0.35f, 1); // kääntää Spriten toiseen suuntaan
        }
        // right
        else {
            sidemove = moveSpeed * Time.deltaTime;
            //transform.localScale = new Vector3(0.35f, 0.35f, 1); // kääntää Spriten toiseen suuntaan
        }
        //if(!landed)
        //    sidemove *= 5;
        transform.Translate(sidemove, 0, 0);
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
    

    bool FindPlayer()
    {
        return (Physics2D.OverlapCircle(rb.position, actdistance, Player));
    }


    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        size = hitbox.bounds.size;
        // default values if nothing set in Unity
        if(stepheight == 0)
            stepheight = 0.05f;
        if(dropheight == 0)
            dropheight = 2.5f;
        if(actdistance < 5f)
            actdistance = 5f;
        landed = true;
    }


    void Update()
    {
        // move around
        if(waitforplayer == false) {
            CheckMoveDir();
            MoveEnemy();
            // this enemy can jump
            if(allowjump && Random.value < 0.01f && IsGrounded() && Time.timeSinceLevelLoad > move_time + 0.5f)
                JumpEnemy();
        }
        // wait for player to get close enough
        else {
            if(FindPlayer())
                waitforplayer = false;
        }
    }


    // hits an invisible trigger, not currently in use, would need to check for specific trigger?
    void OnTriggerEnter2D_notused(Collider2D col)
    {
        if (move_dir == 0)
            move_dir = 1;
        else
            move_dir = 0;
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Untagged")
            return;
        // colliding with another enemy or a wall, turn away
        if(col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wall") {
            ChangeMoveDir();
            return;
        }
        sidemove = moveSpeed * Time.deltaTime;
        if(move_dir == 0) {
            transform.Translate(sidemove, 0, 0);
            //col.gameObject.SendMessage("ApplyDamage", 1);
        }
        else {
            transform.Translate(-sidemove, 0, 0);
            //col.gameObject.SendMessage("ApplyDamage", 1);
        }
        ChangeMoveDir();
        /*
        move_dir = 2;
        move_time = Time.timeSinceLevelLoad + 0.25f;
        JumpEnemy();
        */
    }
}
