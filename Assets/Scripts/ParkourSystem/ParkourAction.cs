using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basically this is placing to access the reference of an animations from unity and create the new parkour actions
[CreateAssetMenu(menuName ="Parkour System/New Parkour Action")]

// Scriptable Objects are Data Container here i'm using them to store the data of parkour actions
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;

    // Now Implementing check object with Tag
    [SerializeField] string obstacleTag;

    //these values are helpful to perform action based on height
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] bool rotateToObstacle;
    [SerializeField] float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0, 1, 0);

    public Quaternion TargetRotation { get; set; }
    public Vector3 matchPosition { get; set; }

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {

        //===================================== Checking with tag ===============================================
        if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
            return false;

        // if the hitObstacle is has height to perform an action then it will minus the object hit leangth from the position of player 
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            matchPosition = hitData.heightHit.point;

        return true;
    }

    // creating properties to use them in another script like parkour controller
    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;


    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
   

}
