using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Platform : MonoBehaviour
{
    public float moveSpeed; // main move speed
    private Vector3 speed; // x/y speed depends on the moveheight/width
    public float moveHeight; // up/down from start position
    public float moveWidth; // left/right from start position
    public float waitTime; // wait time at the start or end position
    public float move_dir;

    private Vector3 spawnorigin;
    private Vector3 endorigin;
    private Vector3 destination;
    private float moveLength;
    private float moveTime;
    private float delay;
    private Collider2D plat;

    [SerializeField] GameObject interactCanvas;
    [SerializeField] TMP_Text interactText;
    bool interactable;
    bool movePlatform;
    [SerializeField] InputHandler inputHandlerScript;

    void ChangeMoveDir()
    {
        // randomize at level startup
        if(move_dir == 0) {
            if(Random.value < 0.5f) {
                move_dir = 1;
                destination = endorigin;
            }
            else {
                move_dir = -1;
                destination = spawnorigin;
            }
        }
    }

    void MovePlatform()
    {
        // waiting at the destination
        //if(Time.timeSinceLevelLoad < delay)
        //    return;

        // calculate moving from the current position to the destination
        moveLength = Vector3.Distance(plat.transform.position, destination);
        moveTime = moveLength / moveSpeed;
        speed = (destination - plat.transform.position) * (1/moveTime);

        // reached destination
        if(moveTime < 0.01f) {
            speed = new Vector3(0, 0, 0);
            transform.position = destination;
            movePlatform = false;
            if(move_dir == 1)
                destination = spawnorigin;
            else
                destination = endorigin;
            move_dir *= -1;
            delay = Time.timeSinceLevelLoad + waitTime;
        }
        // still moving toward the destination
        else
            interactCanvas.SetActive(false);
            transform.Translate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        plat = GetComponent<Collider2D>();
        spawnorigin = plat.transform.position;
        endorigin = plat.transform.position + new Vector3(moveWidth, moveHeight, 0);
        destination = plat.transform.position + new Vector3(moveWidth, moveHeight, 0);
        if(moveSpeed == 0)
            moveSpeed = 8f;
        ChangeMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
       
        if (movePlatform)
        {
            MovePlatform();
        } 
    }

    // move the player or enemies on the platform
    void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.tag == "Enemy" || col.gameObject.tag == "Player") {
            col.gameObject.transform.Translate(speed.x * Time.deltaTime, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            interactCanvas.SetActive(true);
            interactable = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            interactCanvas.SetActive(false);
            interactable = false;
        }
    }

    public void InteractWithElevator(InputAction.CallbackContext context)
    {
        if (context.performed && interactable)
        {
            movePlatform = true;
        }
    }

    //Changes the interaction text based on what the player is playing with
    void CheckForInput()
    {
        if (inputHandlerScript.keyboardActive)
        {
            interactText.text = "[E] Interact";
        }
        else
        {
            interactText.text = "[Y] Interact";
        }

    }
}
