using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AnimatorManager))]
public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    PlayerMovement playerMovement;

    public Vector2 movementInput;
    public Vector2 m_cameraInput;

    public float verticalInput;
    public float horizontalInput;

    public float cameraInputX;
    public float cameraInputY;


    public float movementAmount;

    [Header("Input Buttons Flags")]
    public bool bInput;
    public bool jumpInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => m_cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.B.performed += i => bInput = true;
            playerControls.PlayerActions.B.canceled += i => bInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;
        }
        playerControls.Enable();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpInput();
    }

    void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = m_cameraInput.x;
        cameraInputY = m_cameraInput.y;


        movementAmount = Mathf.Clamp01(MathF.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.ChangeAnimatorValues(0, movementAmount, playerMovement.isSprinting);

    }

    void HandleSprintingInput()
    {
        if (bInput && movementAmount > 0.5f)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }

    void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }



}
