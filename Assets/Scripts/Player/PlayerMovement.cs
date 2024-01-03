using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
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


    // Update is called once per frame
    void Update()
    {
        // horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;


        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        //MilasMovementScript.milasMovementScript.OnMovementPerformed();


        //if (InputAction.CallbackContext context)
        //{
        //    if(jumpsPerformed < maxJumps)
        //    {
        //        jump = true;
        //        animator.SetBool("IsJumping", true);
        //        FindObjectOfType<AudioManager>().Play("Jumping");
        //        jumpsPerformed++;

        //        if(jumpsPerformed == 2)
        //        {
        //            animator.SetTrigger("DoubleJump");
        //        }

        //    }
        //}

        //if (Input.GetButtonDown("Crouch"))
        //{
        //    crouch = true;
        //    animator.SetBool("IsCrouching", true);
        //} else if (Input.GetButtonUp("Crouch"))

        //{
        //    crouch = false;
        //    animator.SetBool("IsCrouching", false);

        //}
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






