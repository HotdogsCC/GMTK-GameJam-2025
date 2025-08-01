using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackBase : MonoBehaviour
{
    public abstract Vector3 GetExit1Pos();

    public abstract Vector3 GetExit2Pos();

    public Vector3 GetBezierPos()
    {
        return bezierTransform.position;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
    
    [SerializeField] protected Transform exitTransform1;
    [SerializeField] protected Transform exitTransform2;
    [SerializeField] protected Transform bezierTransform;
}
