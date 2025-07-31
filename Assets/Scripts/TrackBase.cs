using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackBase : MonoBehaviour
{
    public Vector3 GetExit1Pos()
    {
        return exitTransform1.position;
    }
    
    public Vector3 GetExit2Pos()
    {
        return exitTransform2.position;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
    
    [SerializeField] protected Transform exitTransform1;
    [SerializeField] protected Transform exitTransform2;
}
