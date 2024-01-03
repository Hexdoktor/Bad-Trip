using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Inputs input = null;
    public Vector2 moveVector = Vector2.zero;

    [SerializeField] GameObject gamepadControls;
    [SerializeField] GameObject keyboardControls;
    [SerializeField] PlayerMovement playerMovement;

    private void Awake()
    {
        
        input = new Inputs(); 

    }


    private void Update()
    {
        //Checks whether the player is using a keyboard or controller
        if (Gamepad.current != null && Gamepad.current.allControls.Any(control => control.IsPressed()))
        {       
            gamepadControls.SetActive(true);
            keyboardControls.SetActive(false);

        }
        else if (Keyboard.current.wasUpdatedThisFrame && Keyboard.current.anyKey.isPressed)
        {    
            gamepadControls.SetActive(false);
            keyboardControls.SetActive(true);

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
