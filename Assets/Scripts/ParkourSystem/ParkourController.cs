using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;

    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;
    [SerializeField] ParkourAction jumpDownParkourAction;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        //Using Space Key to Perform Parkour Action

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) && !playerController.inAction && !playerController.playerHanging)
        {
           
            var hitData = environmentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {

                foreach(var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {

                        //Debug.Log("ObstacleFound " + hitData.forwardHit.transform.name);
                        StartCoroutine(PerformParkourAction(action));
                        break;
                    }
                }
            }
        }

        if(playerController.playerOnLedge && !playerController.inAction && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        {
            if(playerController.LedgeInfo.angle <=45)
            {
                playerController.playerOnLedge = false;
                StartCoroutine(PerformParkourAction(jumpDownParkourAction));
            }
            
        }
    }
    //Creating Small Daley while the player in action to finish it
    IEnumerator PerformParkourAction(ParkourAction action)
    {
        playerController.SetControl(false);

        CompareTargetParameter compareTargetParameter = null;
        if(action.EnableTargetMatching)
        {
            compareTargetParameter = new CompareTargetParameter()
            {
                position = action.matchPosition,
                bodyPart = action.MatchBodyPart,
                positionWeight = action.MatchPositionWeight,
                startTime = action.MatchStartTime,
                endTime = action.MatchTargetTime,
            };
        }

        yield return playerController.PerformAction(action.AnimName, compareTargetParameter, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay);

        playerController.SetControl(true);
    }
    
    // this is helpful to match the target position on the hit object at the top point of an object
    void Matchtarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.matchPosition, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPositionWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }

}
