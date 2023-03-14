using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    float sidemove = 0.0f;

    public float moveSpeed;
    public float jumpForce;
    float move_time;
    float move_dir; // 0 = right, 1 = left, 2 = vertical (vertical may be a test/placeholder for attack)

    public Rigidbody2D rb;
    public Vector2 size;
    private Collider2D hitbox;
    private RaycastHit2D hit;

    public Transform groundCheck;
    public LayerMask Ground;
    public LayerMask invisiblock;

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
        Vector2 vec = new Vector2(0, 2.5f);
        hit = Physics2D.Linecast(rb.position, rb.position - vec, Ground);
        if(!hit) {
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
        if(Physics2D.CapsuleCast(rb.position, size*0.95f, CapsuleDirection2D.Vertical, 0, vec, 0.05f, Ground)) {
            // can be stepped up?
            if(!Physics2D.CapsuleCast(rb.position + new Vector2(0, 0.6f), size*0.95f, CapsuleDirection2D.Vertical, 0, vec, 0.05f, Ground)) {
                if(slopeAngle == 0)
                    transform.Translate(0, 0.6f, 0);
            }
            else
                ChangeMoveDir();
        }
    }

    void MoveEnemy()
    {
        // just vertical movement
        if (Time.timeSinceLevelLoad > move_time && move_dir == 2 && IsGrounded()) {
            ChangeMoveDir();
            //move_time = Time.timeSinceLevelLoad + 0.5f + Random.value*4.5f;
        }
        // left
        if (move_dir == 0) {
            sidemove = -moveSpeed * Time.deltaTime;
            //transform.localScale = new Vector3(-0.35f, 0.35f, 1); // kääntää Spriten toiseen suuntaan

            transform.Translate(sidemove, 0, 0);
        }
        // right
        if (move_dir == 1) {
            sidemove = moveSpeed * Time.deltaTime;
            //transform.localScale = new Vector3(0.35f, 0.35f, 1); // kääntää Spriten toiseen suuntaan

            transform.Translate(sidemove, 0, 0);
        }
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
        //if(Random.value < 0.05f)// && IsGrounded())
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }


    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        size = hitbox.bounds.size;
    }


    void Update()
    {
        CheckMoveDir();
        MoveEnemy();
        //JumpEnemy();
    }

    // hits an invisible trigger
    void OnTriggerEnter2D(Collider2D col)
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
        move_dir = 2;
        move_time = Time.timeSinceLevelLoad + 0.25f;
        JumpEnemy();
    }
}
