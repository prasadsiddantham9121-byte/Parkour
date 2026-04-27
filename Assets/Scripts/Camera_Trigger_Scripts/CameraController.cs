using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraSide
    {
        Default,
        Left,
        Right
    }

    [Header("Target")]
    [SerializeField] Transform followTarget;

    [Header("Offset Settings")]
    [SerializeField] Vector3 baseOffset = new Vector3(0, 2, -5);

    [Header("Follow Settings")]
    [SerializeField] float followSmoothTime = 0.2f;
    [SerializeField] float rotationSpeed = 8f;

    [Header("Side Settings")]
    [SerializeField] CameraSide currentSide = CameraSide.Default;
    [SerializeField] float sideAngle = 90f;
    [SerializeField] float angleSmoothSpeed = 5f;

    float currentAngle = 0f;
    float targetAngle = 0f;

    Vector3 velocity;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        // Decide target angle
        switch (currentSide)
        {
            case CameraSide.Default:
                targetAngle = 0f;
                break;
            case CameraSide.Left:
                targetAngle = -sideAngle;
                break;
            case CameraSide.Right:
                targetAngle = sideAngle;
                break;
        }

        // Smooth angle transition
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, angleSmoothSpeed * Time.deltaTime);

        // Rotate offset
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 rotatedOffset = rotation * baseOffset;

        // Smooth follow
        Vector3 targetPosition = followTarget.position + rotatedOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSmoothTime);

        // Always look at player
        Vector3 lookDir = followTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetCameraSide(CameraSide side)
    {
        currentSide = side;
    }

    // ✅ NEW: instant reset (used on respawn)
    public void ResetToDefaultInstant()
    {
        currentSide = CameraSide.Default;
        currentAngle = 0f;
    }

    public Quaternion PlanarRotation
    {
        get
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            return Quaternion.LookRotation(forward);
        }
    }
}