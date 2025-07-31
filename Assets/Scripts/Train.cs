using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("Attributes")] 
    [SerializeField] private float moveSpeed = 1.0f;
    
    [Header("Children")]
    [SerializeField] private Transform raycastStartLocation;

    private Vector3 entrancePosition;
    private Vector3 bezierPosition;
    private Vector3 targetPosition;
    private float t = 0;

    private Quaternion originalRotation;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (raycastStartLocation == null)
        {
            Debug.LogWarning("add the raycast start location to the train");
        }
        
        FindNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        //move towards target
        //float frameMoveSpeed = moveSpeed * Time.deltaTime;
        
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, frameMoveSpeed);
        
        //lerp with bezier
        
        Vector3 newPosition =
            (Mathf.Pow(1 - t, 2) * entrancePosition)
            + (2 * (1 - t) * t * bezierPosition)
            + (Mathf.Pow(t, 2) * targetPosition);
        
        transform.LookAt(newPosition);
        transform.position = newPosition;
        
        t += moveSpeed * Time.deltaTime;
        
        //are we finished?
        if (t >= 1.0f)
        {
            FindNewTarget();
        }
            
        
        //are we at the target?
        //if (transform.position == targetPosition)
        //{
         //   FindNewTarget();
       // }
    }

    private void FindNewTarget()
    {
        //check the track underneath us
        RaycastHit hit;
        if (Physics.Raycast(raycastStartLocation.position, Vector3.down, out hit, 3.0f, 64))
        {
            //get the track class
            TrackBase track = hit.transform.GetComponent<TrackBase>();
            
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

            bezierPosition = track.GetBezierPos();

            t = 0.0f; 
            
            //look at the target
            //transform.LookAt(targetPosition);
        }
    }
}
