using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Carridge : MonoBehaviour
{
    [SerializeField] private GameObject carridgePrefab;
    private Carridge carridgeChild = null;

    private float carridgeLagTime;
    private float moveSpeed;

    [SerializeField] private MeshRenderer mesh;

    [Header("Materials")] 
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material blueMat;

    public TrainColour myTrainColour;

    private bool isDestroying = false;
    private float scale = 1.0f;

    public void SetThisBadBoyUp(float _carLag, float _moveSpeed, TrainColour _colour)
    {
        carridgeLagTime = _carLag;
        moveSpeed = _moveSpeed;
        myTrainColour = _colour;

        switch (myTrainColour)
        {
            case TrainColour.Red:
                mesh.material = redMat;
                break;
            case TrainColour.Green:
                mesh.material = greenMat;
                break;
            case TrainColour.Blue:
                mesh.material = blueMat;
                break;
            
        }
    }
    
    private struct TimeStampData
    {
        public float timeStamp;
        public Vector3 position;
        public Quaternion rotation;
    }

    private Queue<TimeStampData> timeStamps = new Queue<TimeStampData>();

    private void Update()
    {
        if (isDestroying)
        {
            scale -= Time.deltaTime;
            if (scale <= 0.0f)
            {
                Destroy(gameObject);
                return;
            }

            transform.localScale = new Vector3(scale, scale, scale);
            return;
        }
        
        if (carridgeChild)
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
                carridgeChild.transform.position = newTimeStamp.position;
                carridgeChild.transform.rotation = newTimeStamp.rotation;
            }
        }
    }

    public void CreateChild(int numberOfKidsLeft)
    {
        numberOfKidsLeft -= 1;
        if (numberOfKidsLeft > 0) 
        {
            if (carridgeChild == null)
            {
                GameObject carridgeInstance = Instantiate(carridgePrefab, transform.position, transform.rotation);
           
                carridgeChild = carridgeInstance.GetComponent<Carridge>();
                
            }
            carridgeChild.SetThisBadBoyUp(carridgeLagTime, moveSpeed, myTrainColour);
            
            carridgeChild.CreateChild(numberOfKidsLeft);
        }
        
        
        
        
    }

    public void DestroyCarridge()
    {
        if (carridgeChild != null)
        {
            carridgeChild.DestroyCarridge();
        }

        GetComponent<BoxCollider>().enabled = false;

        isDestroying = true;
    }
}
