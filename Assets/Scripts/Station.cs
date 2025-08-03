using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Station : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private TrainColour myColour;
    [SerializeField] private int pointsNeededForActivation = 0;
    [SerializeField] private float minSpawnTime = 5.0f;
    [SerializeField] private float maxSpawnTime = 10.0f;

    [Header("Spawn Area")] 
    [SerializeField] private GameObject[] spawnLocations;
    private TrainColour[] peopleColours;
    private List<TrainColour> possibleColours = new List<TrainColour>();
    private int peopleWaiting = 0;

    [Header("References")] 
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material whiteMat;

    private StationManager stationManager;
    private MeshRenderer mesh;
    

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        stationManager = FindObjectOfType<StationManager>();
        
        if(stationManager.isUsingRed)
            possibleColours.Add(TrainColour.Red);
        if(stationManager.isUsingGreen)
            possibleColours.Add(TrainColour.Green);
        if(stationManager.isUsingBlue)
            possibleColours.Add(TrainColour.Blue);

        if (pointsNeededForActivation == 0)
        {
            StartCoroutine(SpawnPerson());
        }
        
        
        if (stationManager == null)
        {
            Debug.LogWarning("Please add a Station Manager object to the scene");
        }
        
        foreach (var spawnLocation in spawnLocations)
        {
            spawnLocation.SetActive(false);
        }

        peopleColours = new TrainColour[spawnLocations.Length];
        for (int i = 0; i < peopleColours.Length; i++)
        {
            peopleColours[i] = TrainColour.White;
        }
        
        SetTeam(myColour);
    }

    private void OnTriggerEnter(Collider other)
    {
        Train train = other.GetComponent<Train>();

        //is this the same colour as me?
        if (train.GetColour() == myColour)
        {
            stationManager.DropOff(this, (train.GetPeople()));
            train.TakePeople();
        }

        //take the people we need to grab
        for (int i = 0; i < peopleColours.Length; i++)
        {
            //is this the colour of the train?
            if (train.GetColour() == peopleColours[i])
            {
                peopleColours[i] = TrainColour.White;
                peopleWaiting--;
                spawnLocations[i].SetActive(false);
                train.AddPeople(1);
            }
        }
}

    public void SetTeam(TrainColour col)
    {
        if (myColour != TrainColour.White && !possibleColours.Contains(myColour))
        {
            possibleColours.Add(myColour);
        }
        
        myColour = col;
        
        //if there is a person of this colour, remove them them
        for (int i = 0; i < peopleColours.Length; i++)
        {
            //is this the colour of the train?
            if (peopleColours[i] == myColour)
            {
                peopleColours[i] = TrainColour.White;
                peopleWaiting--;
                spawnLocations[i].SetActive(false);
            }
        }
        
        //set the material of the roof
        
        switch (myColour)
        {
            case TrainColour.Red:
                mesh.material = redMat;
                possibleColours.Remove(TrainColour.Red);
                break;
            case TrainColour.Green:
                mesh.material = greenMat;
                possibleColours.Remove(TrainColour.Green);
                break;
            case TrainColour.Blue:
                mesh.material = blueMat;
                possibleColours.Remove(TrainColour.Blue);
                break;
            case TrainColour.White:
                mesh.material = whiteMat;
                break;
        }
    }

    public TrainColour GetColour()
    {
        return myColour;
    }

    public IEnumerator SpawnPerson()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        
        ActuallySpawnTheGuy();
        

        StartCoroutine(SpawnPerson());
    }

    public void ActuallySpawnTheGuy()
    {
        //is there room to spawn a person?
        if (peopleWaiting < spawnLocations.Length && possibleColours.Count != 0)
        {
            //find an empty spot
            for (int i = 0; i < peopleColours.Length; i++)
            {
                //is the slot empty?
                if (peopleColours[i] == TrainColour.White)
                {
                    //add another person waiting
                    peopleWaiting++;
                    //get a random colour for the person to spawn
                    peopleColours[i] = possibleColours[Random.Range(0, possibleColours.Count)];
                    //set the colour for that person
                    spawnLocations[i].SetActive(true);
                    spawnLocations[i].GetComponent<MeshRenderer>().material = GetMaterialFromColour(peopleColours[i]);

                    break;
                }
            }
        }
    }

    private Material GetMaterialFromColour(TrainColour colour)
    {
        switch (colour)
        {
            case TrainColour.Red:
                return redMat;
            case TrainColour.Green:
                return greenMat;
            case TrainColour.Blue:
                return blueMat;
        }

        return whiteMat;
    }

    public int GetPointsNeeded()
    {
        return pointsNeededForActivation;
    }

    public void AddRed()
    {
        possibleColours.Add(TrainColour.Red);
    }
}
