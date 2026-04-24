using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 2.5f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 6f;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Check Ledge")]
    [SerializeField] float ledgeRayLength = 12f;
    [SerializeField] float ledgeRayHeightThreshold = 0.76f;

    [Header("Climbing Check")]
    [SerializeField] float climbingRayLength = 1.6f;
    [SerializeField] LayerMask climbingLayer;
    public int numberOfRays = 12;

    public ObstacleHitData ObstacleCheck()
    {
        // Here the reference has been tken for hitdata
        var hitData = new ObstacleHitData();


        // this ray cast is using for Obstacle check wheather it obstacle or not
        var forwardOrigin = transform.position + forwardRayOffset;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, out hitData.forwardHit,
           forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.green);

        // Performing Height (vertical) RayCast here When player hit any Obstcle

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound =  Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.green);

        }

        return hitData;
    }

    public bool CheckLedge(Vector3 movementDirection, out LedgeInfo ledgeInfo)
    {
        ledgeInfo = new LedgeInfo();

        if(movementDirection == Vector3.zero)
             return false;

        float ledgeOriginOffset = 0.5f;
        var ledgeOrigin = transform.position + movementDirection * ledgeOriginOffset + Vector3.up;

        if(Physics.Raycast(ledgeOrigin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleLayer))
        {

            Debug.DrawRay(ledgeOrigin, Vector3.down * ledgeRayLength, Color.blue);

            var surfaceRaycastorigin = transform.position + movementDirection - new Vector3(0, 0.1f, 0);

            if(Physics.Raycast(surfaceRaycastorigin, -movementDirection, out RaycastHit surfaceHit, 2, obstacleLayer))
            {
                float Ledgeheight = transform.position.y - hit.point.y;

                if (Ledgeheight > ledgeRayHeightThreshold)
                {
                    ledgeInfo.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeInfo.height = Ledgeheight;
                    ledgeInfo.surfacehit = surfaceHit;
                    return true;
                }
            }
            
        }
        return false;
    }


    // this is for Checking the climb points

    public bool CheckClimbing(Vector3 climbDirection, out RaycastHit climbInfo)
    {
        climbInfo = new RaycastHit();

        if (climbDirection == Vector3.zero)
            return false;

        var climbOrigin = transform.position + Vector3.up * 1.5f;
        var climbOffset = new Vector3(0, 0.19f, 0);

        for(int i = 0; i < numberOfRays; i++)
        {
            Debug.DrawRay(climbOrigin + climbOffset * i, climbDirection, Color.red);
            if(Physics.Raycast(climbOrigin + climbOffset * i, climbDirection, out RaycastHit hit, climbingRayLength, climbingLayer))
            {
                climbInfo = hit;
                return true;
            }
        }
        return false;
    }

    // this function is about to find the nearest climb point when the player is inroof top to get down
    //public bool CheckDropClimbPoint(out RaycastHit DropHit)
    //{
    //    DropHit = new RaycastHit();

    //    var origin = transform.position + Vector3.down * 0.1f + transform.forward * 2f;

    //    if(Physics.Raycast(origin, - transform.forward, out RaycastHit hit, 3, climbingLayer) )
    //    {
    //        DropHit = hit;
    //        return true;
    //    }

    //    return false;

    //}

    public bool CheckDropClimbPoint(out RaycastHit dropHit)
    {
        dropHit = new RaycastHit();

        // Step 1: Start slightly forward and above player
        Vector3 origin = transform.position + Vector3.up * 1.5f + transform.forward * 0.5f;

        float downDistance = 3f;

        // Step 2: Cast DOWN to find ledge
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, downDistance, climbingLayer))
        {
            Debug.DrawRay(origin, Vector3.down * downDistance, Color.green);

            dropHit = hit;
            return true;
        }

        Debug.DrawRay(origin, Vector3.down * downDistance, Color.red);
        return false;
    }

}

// Struct is a Data Container it will Store All the Data the ray Cast hit
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct LedgeInfo
{
    public float angle;
    public float height;
    public RaycastHit surfacehit;
}
