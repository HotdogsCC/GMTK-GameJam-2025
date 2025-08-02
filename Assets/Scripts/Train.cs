using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TrainColour
{
    Red,
    Green,
    Blue,
    White
}

public class Train : MonoBehaviour
{
    [Header("Attributes")] 
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private TrainColour myTrainColour;
    //scaled move speed based on the distance
    private float actualMoveSpeed;
    
    [Header("raycast")]
    [SerializeField] private Transform raycastStartLocation;

    [Header("Carridges")] 
    [SerializeField] private int peoplePerCarriage = 4;
    [SerializeField] int carridgeAmount = 0;
    [SerializeField] float carridgeLagTime = 0.45f;
    [SerializeField] GameObject carridgePrefab;
    private Carridge carridge;

    private Vector3 entrancePosition;
    private Vector3 bezierPosition;
    private Vector3 targetPosition;
    private float t = 0;
    private int people = 0;

    

    //used for a series of speeds based on where the train is
    private float[] speeds = new float[100];
    
    //used for data about the carridge
    private struct TimeStampData
    {
        public float timeStamp;
        public Vector3 position;
        public Quaternion rotation;
    }

    private Queue<TimeStampData> timeStamps = new Queue<TimeStampData>();

    private float mercyTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (raycastStartLocation == null)
        {
            Debug.LogWarning("add the raycast start location to the train");
        }

        if (carridgeAmount > 0)
        {
            GameObject carridgeInstance = Instantiate(carridgePrefab, transform.position, transform.rotation);
           
            carridge = carridgeInstance.GetComponent<Carridge>();
            carridge.SetThisBadBoyUp(carridgeLagTime, moveSpeed, myTrainColour);
            carridge.CreateChild(carridgeAmount);
        }

        actualMoveSpeed = moveSpeed;
        
        FindNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
        mercyTime -= Time.deltaTime;
        if (mercyTime <= 0.0f)
        {
            mercyTime = 0.0f;
        }
        
        Vector3 newPosition =
            (Mathf.Pow(1 - t, 2) * entrancePosition)
            + (2 * (1 - t) * t * bezierPosition)
            + (Mathf.Pow(t, 2) * targetPosition);
        
        transform.LookAt(newPosition);
        transform.position = newPosition;
        
        t += GetCurrentSpeed(t) * Time.deltaTime;
        
        //are we finished?
        if (t >= 1.0f)
        {
            FindNewTarget();
        }
        
        

        if (carridge)
        {
            timeStamps.Enqueue(new TimeStampData
            {
                timeStamp = Time.time,
                position = transform.position,
                rotation = transform.rotation
            });
            
            TimeStampData newTimeStamp = default;
            bool iDidAThing = false;
            while (timeStamps.Count != 0 && timeStamps.Peek().timeStamp + (carridgeLagTime / moveSpeed) < Time.time)
            {
                iDidAThing = true;
                newTimeStamp = timeStamps.Dequeue();
            }

            if (iDidAThing)
            {
                carridge.transform.position = newTimeStamp.position;
                carridge.transform.rotation = newTimeStamp.rotation;
            }
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
            if(track.TryGetComponent<SwitchingTrainTrack>(out switchingTrack))
            {
                //are we closer to in the inactive track?
                //i.e. are we travelling into the merge
                if(Vector3.Distance(transform.position, switchingTrack.GetInactiveExit()) 
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
            if(track.TryGetComponent<IntersectionTrack>(out intersection))
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
        //break the curve into 100 segements to guesstimate the length
        for (int i = 1; i <= 100; i++)
        {
            percentage = i * 0.01f;

            currentPosition =
            (Mathf.Pow(1 - percentage, 2) * entrancePosition)
            + (2 * (1 - percentage) * percentage * bezierPosition)
            + (Mathf.Pow(percentage, 2) * targetPosition);

            calculatedDistance += Vector3.Distance(previousPosition, currentPosition);

            speeds[i - 1] = (0.01f / Vector3.Distance(previousPosition, currentPosition)) * moveSpeed;

            //Debug.Log(Vector3.Distance(previousPosition, currentPosition));

            previousPosition = currentPosition;
        }

        actualMoveSpeed = (1.0f / calculatedDistance) * moveSpeed;
    }
    
    private float GetCurrentSpeed(float myT)
    {
        myT *= 100;
        int intT = Mathf.RoundToInt(myT);

        if (intT >= 100)
        {
            intT = 99;
        }

        return speeds[intT];
    }

    private void OnTriggerEnter(Collider other)
    {
        //if didnt collied with a train
        if (other.gameObject.layer != 7)
        {
            return;
        }
        
        if (mercyTime > 0.0f)
        {
            return;
        }
        
        Debug.Log("Game over!");

        StaticObjectHolder.theGameManager.EndGame();
    }

    private void CreateCarridge()
    {
        carridgeAmount++;
        mercyTime = 1.0f;

        //if we dont have a carridge
        if (carridge == null)
        { 
            GameObject carridgeInstance = Instantiate(carridgePrefab, transform.position, transform.rotation);

            carridge = carridgeInstance.GetComponent<Carridge>();
            carridge.SetThisBadBoyUp(carridgeLagTime, moveSpeed, myTrainColour);
        }
        else
        {
            carridge.CreateChild(carridgeAmount);
            carridge.SetThisBadBoyUp(carridgeLagTime, moveSpeed, myTrainColour);
        }
        
        
        
       
        
    }

    private void DestoryCarridges()
    {
        carridgeAmount = 0;
        timeStamps.Clear();
        if (carridge != null)
        {
            carridge.DestroyCarridge();
            carridge = null;
        }
    }
    
    public TrainColour GetColour()
    {
        return myTrainColour;
    }

    public int GetPeople()
    {
        return people;
    }

    public void AddPeople(int toAdd)
    {
        people += toAdd;

        //figure out the amount of carriages we should have
        int desiredCarriages = 1;
        int tempPeople = people;
        while (tempPeople > peoplePerCarriage)
        {
            tempPeople -= peoplePerCarriage;
            desiredCarriages++;
        }

        //do we have the right amount  of carriages?
        if (desiredCarriages != carridgeAmount)
        {
            desiredCarriages -= carridgeAmount;

            while (desiredCarriages > 0)
            {
                CreateCarridge();
                desiredCarriages -= 1;
            }
        }
        
    }

    public void TakePeople()
    {
        people = 0;
        DestoryCarridges();
    }
}
