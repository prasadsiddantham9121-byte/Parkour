using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    public EnvironmentScanner environmentScanner;
    public PlayerController playerController;

    ClimbingPoint currentClimbPoint;

    public float InOutValue;
    public float UpDownValue;
    public float LeftRightValue;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        bool jumpPressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
        bool leavePressed = Input.GetButton("Leave") || Input.GetKey(KeyCode.Menu);

        if (!playerController.playerHanging)
        {
            // --------- CLIMB FROM GROUND ---------
            if (jumpPressed && !playerController.inAction)
            {
                if (environmentScanner.CheckClimbing(transform.forward, out RaycastHit climbInfo))
                {
                    currentClimbPoint = climbInfo.transform.GetComponent<ClimbingPoint>();

                    playerController.SetControl(false);

                    InOutValue = -0.30f;
                    UpDownValue = -0.9f;
                    LeftRightValue = 0.15f;

                    StartCoroutine(ClimbToLedge("IdleToClimb", climbInfo.transform, 0.40f, 0.54f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
            }

            // --------- DROP TO HANG ---------
            if (leavePressed && !playerController.inAction)
            {
                if (environmentScanner.CheckDropClimbPoint(out RaycastHit dropHit))
                {
                    currentClimbPoint = GetNearestClimbingPoint(dropHit.transform, dropHit.point);

                    playerController.SetControl(false);

                    InOutValue = 0f;
                    UpDownValue = -0.15f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(ClimbToLedge("DropToFreehang", currentClimbPoint.transform, 0.41f, 0.54f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
            }
        }
        else
        {
            // --------- JUMP FROM WALL ---------
            if (leavePressed && !playerController.inAction)
            {
                StartCoroutine(JumpFromWall());
                return;
            }

            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));

            var inputDirection = new Vector2(horizontal, vertical);

            if (playerController.inAction || inputDirection == Vector2.zero)
                return;

            // --------- WALL TO TOP (FIXED) ---------
            if (currentClimbPoint.MountPoint && inputDirection.y == 1 && jumpPressed)
            {
                StartCoroutine(WallToTop());
                return;
            }

            // --------- LEDGE MOVEMENT ---------
            var neighbour = currentClimbPoint.GetNeighbour(inputDirection);

            if (neighbour == null) return;

            // --------- JUMP BETWEEN LEDGES ---------
            if (neighbour.connectionType == ConnectionType.Jump && jumpPressed)
            {
                currentClimbPoint = neighbour.climbingPoint;

                if (neighbour.pointDirection.y == 1)
                {
                    InOutValue = 0.15f;
                    UpDownValue = 0.09f;
                    LeftRightValue = 0.30f;

                    StartCoroutine(ClimbToLedge("WallUp", currentClimbPoint.transform, 0.44f, 0.64f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
                else if (neighbour.pointDirection.y == -1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.15f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(ClimbToLedge("WallDown", currentClimbPoint.transform, 0.31f, 0.68f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
                else if (neighbour.pointDirection.x == 1)
                {
                    StartCoroutine(ClimbToLedge("WallRight", currentClimbPoint.transform, 0.20f, 0.51f));
                }
                else if (neighbour.pointDirection.x == -1)
                {
                    InOutValue = 0.18f;
                    UpDownValue = 0.09f;
                    LeftRightValue = 0.30f;

                    StartCoroutine(ClimbToLedge("WallLeft", currentClimbPoint.transform, 0.20f, 0.51f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
            }
            // --------- SHIMMY ---------
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentClimbPoint = neighbour.climbingPoint;

                if (neighbour.pointDirection.x == 1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.04f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(ClimbToLedge("ShimmyRight", currentClimbPoint.transform, 0f, 0.30f,
                        playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
                else if (neighbour.pointDirection.x == -1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.04f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(ClimbToLedge("ShimmyLeft", currentClimbPoint.transform, 0f, 0.30f,
                        AvatarTarget.LeftHand,
                        new Vector3(InOutValue, UpDownValue, LeftRightValue)));
                }
            }
        }
    }

    IEnumerator ClimbToLedge(string animationName, Transform ledgePoint, float compareStartTime, float compareEndTime,
        AvatarTarget hand = AvatarTarget.RightHand, Vector3? playerHandOffset = null)
    {
        var compareParams = new CompareTargetParameter()
        {
            position = SetHandPosition(ledgePoint, hand, playerHandOffset),
            bodyPart = hand,
            positionWeight = Vector3.one,
            startTime = compareStartTime,
            endTime = compareEndTime,
        };

        var requiredRot = Quaternion.LookRotation(-ledgePoint.forward);

        yield return playerController.PerformAction(animationName, compareParams, requiredRot, true);
        playerController.playerHanging = true;
    }

    Vector3 SetHandPosition(Transform ledge, AvatarTarget hand, Vector3? playerHandOffset)
    {
        var offset = playerHandOffset ?? new Vector3(InOutValue, UpDownValue, LeftRightValue);

        var handDirection = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;

        return ledge.position + ledge.forward * offset.x + Vector3.up * offset.y - handDirection * offset.z;
    }

    IEnumerator JumpFromWall()
    {
        playerController.playerHanging = false;

        yield return playerController.PerformAction("JumpFromWall");

        playerController.ResetRequiredRotation();
        playerController.SetControl(true);
    }

    IEnumerator WallToTop()
    {
        playerController.playerHanging = false;

        yield return playerController.PerformAction("WallToRoof");

        playerController.EnableCC(true);

        yield return new WaitForSeconds(0.5f);

        playerController.ResetRequiredRotation();
        playerController.SetControl(true);
    }

    ClimbingPoint GetNearestClimbingPoint(Transform dropClimbPoint, Vector3 hitPoint)
    {
        var points = dropClimbPoint.GetComponentsInChildren<ClimbingPoint>();

        ClimbingPoint nearestPoint = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var point in points)
        {
            float dist = Vector3.Distance(point.transform.position, hitPoint);

            if (dist < nearestDistance)
            {
                nearestPoint = point;
                nearestDistance = dist;
            }
        }

        return nearestPoint;
    }
}