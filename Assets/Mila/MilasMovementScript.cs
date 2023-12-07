using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class MilasMovementScript : MonoBehaviour
{
    public Inputs input = null;
    public Vector2 moveVector = Vector2.zero;
    [SerializeField] Rigidbody2D rb;
    public float moveSpeed = 10;

    public bool jump;
    public bool jumpReleased;

    public float jumpForce = 10f;

    [SerializeField] PlayerMovement playerMovement;


    private void Awake()
    {
        
        input = new Inputs();
   
    }
   
    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;

    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
    }

    public void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
        Debug.Log("Analog Stick Input: " + moveVector);
        playerMovement.horizontalMove = value.ReadValue<Vector2>().x * playerMovement.runSpeed;
    }

    public void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;

        playerMovement.horizontalMove = 0f;
    }

}
