using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    public int maxJumps = 2;
    private int jumpsPerformed = 0;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    
    

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        
        if (Input.GetButtonDown("Jump"))
        {
            if(jumpsPerformed < maxJumps)
            {
                jump = true;
                animator.SetBool("IsJumping", true);
                FindObjectOfType<AudioManager>().Play("Jumping");
                jumpsPerformed++;
                
                if(jumpsPerformed == 2)
                {
                    animator.SetTrigger("DoubleJump");
                }

            }
        }
   
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
            animator.SetBool("IsCrouching", true);
        } else if (Input.GetButtonUp("Crouch"))
        
        {
            crouch = false;
            animator.SetBool("IsCrouching", false);
        
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






