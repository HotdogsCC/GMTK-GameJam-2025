using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTrack : TrackBase
{
    [SerializeField] private Transform exitTransform3;
    [SerializeField] private Transform exitTransform4;

    public Vector3 GetEnterancePoint(Vector3 playerPosition)
    {
        //figure out where the player is closest to
        Vector3 closestPosition = exitTransform1.position;
        float smallestDistance = Vector3.Distance(playerPosition, exitTransform1.position);

        float testDistance = Vector3.Distance(playerPosition, exitTransform2.position);
        if (testDistance < smallestDistance)
        {
            smallestDistance = testDistance;
            closestPosition = exitTransform2.position;
        }

        testDistance = Vector3.Distance(playerPosition, exitTransform3.position);
        if (testDistance < smallestDistance)
        {
            smallestDistance = testDistance;
            closestPosition = exitTransform3.position;
        }

        testDistance = Vector3.Distance(playerPosition, exitTransform4.position);
        if (testDistance < smallestDistance)
        {
            smallestDistance = testDistance;
            closestPosition = exitTransform4.position;
        }

        return closestPosition;
    }

    public Vector3 GetExitPoint(Vector3 playerPosition)
    {
        Vector3 enterancePoint = GetEnterancePoint(playerPosition);

        if(enterancePoint == exitTransform1.position)
        {
            return exitTransform2.position;
        }
        if (enterancePoint == exitTransform2.position)
        {
            return exitTransform1.position;
        }

        if (enterancePoint == exitTransform3.position)
        {
            return exitTransform4.position;
        }
        if (enterancePoint == exitTransform4.position)
        {
            return exitTransform3.position;
        }

        return Vector3.zero;
    }

    public override Vector3 GetExit1Pos()
    {
        return exitTransform1.position;
    }

    public override Vector3 GetExit2Pos()
    {
        return exitTransform2.position;
    }
}
