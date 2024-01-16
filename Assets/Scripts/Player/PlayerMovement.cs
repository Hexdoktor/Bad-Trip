using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    public int maxJumps = 2;
    private int jumpsPerformed = 0;

    public float horizontalMove = 0f;
    public bool jump = false;
    bool crouch = false;
    public bool glide;

    [SerializeField] Rigidbody2D rb;
    float initialGravity = 5.5f;
    float glidingSpeed = 1;
    [SerializeField] float fallingSpeedGlide;
    bool endGlide;
  

    // Update is called once per frame
    void Update()
    {
        // horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));


        if (glide) 
        {
            rb.gravityScale = fallingSpeedGlide;
            rb.velocity = new Vector2(rb.velocity.x, glidingSpeed);
            animator.SetBool("IsGliding", true);  
            animator.SetBool("IsJumping", false);
            endGlide = false;
           
        }         
        else 
        {
            if (!endGlide)
            {
                endGlide = true;
                animator.Play("mancoon_flying", 0, 0.66f);
            }
            rb.gravityScale = initialGravity;
            animator.SetBool("IsGliding", false);
           
        }
    }

    public void ActiveGliding()
    {   
        if (glide)
        {
        
            animator.Play("mancoon_flying", 0, 0.23f);
        }
        else
        {
            animator.Play("mancoon_flying", 0, 0.66f);
        }
      
    }
    public void Glide(InputAction.CallbackContext context)
    {
        if (glide && context.performed)
        {
            glide = false;
            animator.Play("mancoon_flying", 0, 0.66f);
            rb.gravityScale = initialGravity;
            animator.SetBool("IsGliding", false);

        }
        else if (rb.velocity.y <= 7 && context.performed && !controller.m_Grounded)
        {
            
            glide = true;
            
        }
     
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouch = true;
            animator.SetBool("IsCrouching", true);
        }
        else
            {
                crouch = false;
                animator.SetBool("IsCrouching", false);

            }
        
    }

    public void Jump(InputAction.CallbackContext context)
        {
        

            if (context.performed)
            {
                if (jumpsPerformed < maxJumps)
                {
               
                    jump = true;
                    animator.SetBool("IsJumping", true);
                    FindObjectOfType<AudioManager>().Play("Jumping");
                    jumpsPerformed++;

                    if (jumpsPerformed == 2)
                    {
                        animator.SetTrigger("DoubleJump");
                    }

                }
            }

        }


        public void OnLanding()
        {
            glide = false;
            animator.SetBool("IsJumping", false);
            jumpsPerformed = 0;
            FindObjectOfType<AudioManager>().Play("Landing");


        }

        void FixedUpdate()
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);

            jump = false;
        }
    }






