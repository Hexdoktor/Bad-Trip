using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MilasMovementScript : MonoBehaviour
{
    public NewControls input = null;
    public Vector2 moveVector = Vector2.zero;
    [SerializeField] Rigidbody2D rb;
    public float moveSpeed = 10;

    public bool jump;
    public bool jumpReleased;

    [SerializeField] PlayerMovement playerMovement;

  //  public static MilasMovementScript milasMovementScript;

    private void Awake()
    {
        
        input = new NewControls();
    }
    public void bruh()
    {
        Debug.Log("dammit");
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
        input.Player.Jump.performed += Jumped;
        input.Player.Jump.canceled -= Jumped;
     //   input.Player.Jump.performed += ctx => jumpReleased = true;

        //    input.Player.Jump.canceled -= ctx => jump = false;

    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
    }

    private void FixedUpdate()
    {
       rb.velocity = moveVector * moveSpeed;
        
            
    }
    public void Jumped(InputAction.CallbackContext value)
    {
        
       playerMovement.OnJump();
    }

    public void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    public void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }
  


}
