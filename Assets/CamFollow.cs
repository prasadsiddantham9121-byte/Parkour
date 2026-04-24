using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform followTarget;

    [Header("Offset Settings")]
    [SerializeField] Vector3 defaultOffset = new Vector3(0, 2, -5);
    [SerializeField] float sideOffsetAmount = 2f; // how much camera shifts left/right

    [Header("Follow Settings")]
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float rotationSpeed = 10f;

    float currentSideOffset;

    private void LateUpdate()
    {
        if (followTarget == null) return;

        // Get player input
        float horizontal = Input.GetAxis("Horizontal");

        // Smooth side movement
        float targetSideOffset = horizontal * sideOffsetAmount;
        currentSideOffset = Mathf.Lerp(currentSideOffset, targetSideOffset, Time.deltaTime * 5f);

        // Final offset with side movement
        Vector3 finalOffset = defaultOffset + new Vector3(currentSideOffset, 0, 0);

        // Convert offset relative to player direction
        Vector3 targetPosition = followTarget.position + followTarget.TransformDirection(finalOffset);

        // Smooth position follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Look at player
        Vector3 direction = followTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
