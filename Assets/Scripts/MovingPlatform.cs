using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private WayPointPath wayPointPath;

    [SerializeField]
    private float _speed;

    private int _targetWayPointIndex;

    private Transform _previousWayPoint;
    private Transform _targetWayPoint;

    private float _timeToWayPoint;
    private float _elapsedTime;

    private void Start()
    {
        TargetNextWayPoint();
    }

    private void FixedUpdate()
    {
        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWayPoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWayPoint.position, _targetWayPoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(_previousWayPoint.rotation, _targetWayPoint.rotation, elapsedPercentage);


        if (elapsedPercentage >= 1)
        {
            TargetNextWayPoint();
        }
    }

    private void TargetNextWayPoint()
    {
        _previousWayPoint = wayPointPath.GetWayPoint(_targetWayPointIndex);
        _targetWayPointIndex = wayPointPath.GetNextWayPointIndex(_targetWayPointIndex);
        _targetWayPoint = wayPointPath.GetWayPoint(_targetWayPointIndex);

        _elapsedTime = 0;

        float distanceToWayPoint = Vector3.Distance(_previousWayPoint.position, _targetWayPoint.position);
        _timeToWayPoint = distanceToWayPoint / _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
