using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carridge : MonoBehaviour
{
    [SerializeField] private Transform raycastStartLocation;
    private float moveSpeed;
    private float actualMoveSpeed;
    private Vector3 entrancePosition;
    private Vector3 bezierPosition;
    private Vector3 targetPosition;
    private float t = 0;
    private bool isRunning = false;

    public void EnableRunning()
    {
        isRunning = true;
    }

    

    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;

        FindNewTarget();
    }

    public void MoveCar()
    {
        if(!isRunning)
        {
            return;
        }

        Vector3 newPosition =
            (Mathf.Pow(1 - t, 2) * entrancePosition)
            + (2 * (1 - t) * t * bezierPosition)
            + (Mathf.Pow(t, 2) * targetPosition);

        transform.LookAt(newPosition);
        transform.position = newPosition;

        t += actualMoveSpeed * Time.deltaTime;

        //are we finished?
        if (t >= 1.0f)
        {
            FindNewTarget();
        }
    }

    private void FindNewTarget()
    {
        //check the track underneath us
        RaycastHit hit;
        if (Physics.Raycast(raycastStartLocation.position, Vector3.down, out hit, 3.0f, 64))
        {
            //get the track class
            TrackBase track = hit.transform.GetComponent<TrackBase>();

            bezierPosition = track.GetBezierPos();

            t = 0.0f;

            //is it a switching track?
            SwitchingTrainTrack switchingTrack;
            if (track.TryGetComponent<SwitchingTrainTrack>(out switchingTrack))
            {
                //are we closer to in the inactive track?
                //i.e. are we travelling into the merge
                if (Vector3.Distance(transform.position, switchingTrack.GetInactiveExit())
                    < Vector3.Distance(transform.position, switchingTrack.GetExit2Pos())
                    &&
                    Vector3.Distance(transform.position, switchingTrack.GetInactiveExit())
                    < Vector3.Distance(transform.position, switchingTrack.GetExit1Pos()))

                {
                    //travel into the merge
                    targetPosition = track.GetExit1Pos();
                    entrancePosition = switchingTrack.GetInactiveExit();

                    SetActualMoveSpeed();

                    return;
                }

            }

            //is it an intersection?
            IntersectionTrack intersection;
            if (track.TryGetComponent<IntersectionTrack>(out intersection))
            {
                entrancePosition = intersection.GetEnterancePoint(transform.position);
                targetPosition = intersection.GetExitPoint(transform.position);

                SetActualMoveSpeed();

                return;
            }

            //see which target is further away
            float exit1Distance = Vector3.Distance(transform.position, track.GetExit1Pos());
            float exit2Distance = Vector3.Distance(transform.position, track.GetExit2Pos());

            //if exit 1 is further away, travel there
            if (exit1Distance > exit2Distance)
            {
                targetPosition = track.GetExit1Pos();
                entrancePosition = track.GetExit2Pos();
            }
            else
            {
                targetPosition = track.GetExit2Pos();
                entrancePosition = track.GetExit1Pos();
            }

            SetActualMoveSpeed();

        }
    }

    private void SetActualMoveSpeed()
    {
        float calculatedDistance = 0.0f;

        Vector3 previousPosition = entrancePosition;
        Vector3 currentPosition;

        float percentage = 0.0f;
        //break the curve into 10 segements to guesstimate the length
        for (int i = 1; i <= 100; i++)
        {
            percentage = i * 0.01f;

            currentPosition =
            (Mathf.Pow(1 - percentage, 2) * entrancePosition)
            + (2 * (1 - percentage) * percentage * bezierPosition)
            + (Mathf.Pow(percentage, 2) * targetPosition);

            calculatedDistance += Vector3.Distance(previousPosition, currentPosition);

            previousPosition = currentPosition;
        }

        actualMoveSpeed = (1.0f / calculatedDistance) * moveSpeed;
    }
}
