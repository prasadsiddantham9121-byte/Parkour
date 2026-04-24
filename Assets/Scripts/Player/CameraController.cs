using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform followTarget;

    [Header("Offset Settings")]
    [SerializeField] Vector3 offset = new Vector3(0, 2, -5);

    [Header("Follow Settings")]
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float rotationSpeed = 10f;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        // Desired position
        Vector3 targetPosition = followTarget.position + offset;

        // Smooth position follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Look at target smoothly
        Vector3 direction = followTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Optional: If your player script still needs it
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