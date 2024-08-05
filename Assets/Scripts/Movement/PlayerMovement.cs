using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;

    Transform cameraTransform;
    Rigidbody rigidbody;

    [Header("Locomotion Flags")]
    public bool isSprinting;
    public bool isMoving;
    public bool isGrounded;
    public bool isJumping;

    [Header("Falling & Landing Motion")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastHeightOffset = 0.5f;
    public LayerMask groundLayer;


    [Header("Locomotion Movement")]
    public float moveSpeed = 1f;
    public float rotationSpeed = 13f;
    public float sprintingSpeed = 7f;

    [Header("Jump Variable")]
    public float jumpHeight = 4f;
    public float gravityIntensity = -15f;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        rigidbody = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    public void HandleAllLocomotion()
    {

        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;

        MovementHandler();
        RotationHandler();
    }

    void MovementHandler()
    {
        if(isJumping)
            return;
            
        moveDirection = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameraTransform.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;
        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.movementAmount >= 0.5f)
            {
                moveDirection = moveDirection * moveSpeed;
                isMoving = true;
            }
            if (inputManager.movementAmount >= 0)
            {
                isMoving = false;
            }
        }

        Vector3 movementVelocity = moveDirection;
        rigidbody.velocity = movementVelocity;
    }

    void RotationHandler()
    {
        if(isJumping)
            return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraTransform.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraTransform.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        Vector3 targetPosition;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            rigidbody.AddForce(transform.forward * leapingVelocity);
            rigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);

        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Landing", true);
            }

            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }

    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            rigidbody.velocity = playerVelocity;

            isJumping = false;
        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }



}
