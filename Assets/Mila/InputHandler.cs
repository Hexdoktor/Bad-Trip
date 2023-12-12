using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Inputs input = null;
    public Vector2 moveVector = Vector2.zero;
    public float moveSpeed = 10;

    public bool jump;
    public bool jumpReleased;
    private bool wasKeyboardPressed = false;
    private bool wasControllerPressed = false;

    public float jumpForce = 10f;

    [SerializeField] GameObject gamepadControls;
    [SerializeField] GameObject keyboardControls;

    [SerializeField] PlayerMovement playerMovement;

   // var isKeyboard;

    private void Awake()
    {
        
        input = new Inputs(); 

    }


    private void Update()
    {
        var keyboardPressed = Keyboard.current.wasUpdatedThisFrame && Keyboard.current.anyKey.isPressed;
        var controllerPressed = Gamepad.current.wasUpdatedThisFrame && Gamepad.current.allControls.Any(control => control.IsPressed());

        if (keyboardPressed && !wasKeyboardPressed)
        {
           
            wasKeyboardPressed = true;
            wasControllerPressed = false;
            gamepadControls.SetActive(false);
            keyboardControls.SetActive(true);
        }
        else if (controllerPressed && !wasControllerPressed)
        {
     
            wasControllerPressed = true;
            wasKeyboardPressed = false;
            gamepadControls.SetActive(true);   
            keyboardControls.SetActive(false);
        }
      
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
        playerMovement.horizontalMove = value.ReadValue<Vector2>().x * playerMovement.runSpeed;
    }

    public void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;

        playerMovement.horizontalMove = 0f;
    }

}
