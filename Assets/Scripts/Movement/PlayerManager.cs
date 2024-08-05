using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerMovement playerMovement;
    CameraManager cameraManager;

    public bool isInteracting;
    private Animator animator;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraManager = FindObjectOfType<CameraManager>();

        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerMovement.HandleAllLocomotion();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMotion();

        isInteracting = animator.GetBool("isInteracting");
        playerMovement.isJumping = animator.GetBool("isJumping");
        animator.SetBool("IsGrounded", playerMovement.isGrounded);
    }


}
