using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform player;
    public Transform cameraTransform;

    [Header("Camera Movement")]

    public Transform cameraPivot;

    private Vector3 followVelocity = Vector3.zero;

    public float followSpeed = 0.3f;
    public float camLookSpeed = 2f;
    public float camPivotSpeed = 2f;

    public float lookAngle;

    public float pivotAngle;

    public float minPivotAngle = -30f;
    public float maxPivotAngle = 30f;

    [Header("Camera Collision")]
    public LayerMask collisionLayer;
    private float defaultPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2f;

    private Vector3 cameraVectorPosition;

    private void Awake()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputManager = FindObjectOfType<InputManager>();
        player = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;


    }

    public void HandleAllCameraMotion()
    {
        FollowTarget();
        RotateCamera();
        CameraCollision();
    }

    void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, player.position, ref followVelocity, followSpeed);
        transform.position = targetPosition;

    }

    void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        pivotAngle = pivotAngle + (inputManager.cameraInputY * camPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    void CameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 dir = cameraTransform.position - cameraPivot.position;
        dir.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, dir, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition =  cameraVectorPosition;

    }


}
