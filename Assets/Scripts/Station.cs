using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Station : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private TrainColour myColour;
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
        possibleColours.Add(TrainColour.Red);
        possibleColours.Add(TrainColour.Green);
        possibleColours.Add(TrainColour.Blue);
        
        StartCoroutine(SpawnPerson());
        
        mesh = GetComponent<MeshRenderer>();
        stationManager = FindObjectOfType<StationManager>();
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
        
        //set the material of the roof
        
        switch (myColour)
        {
            case TrainColour.Red:
                mesh.material = redMat;
                possibleColours.Remove(TrainColour.Red);
                
                //if there is a person of this colour, change them
                for (int i = 0; i < peopleColours.Length; i++)
                {
                    //is this the colour of the train?
                    if (peopleColours[i] == TrainColour.Red)
                    {
                        peopleColours[i] = TrainColour.Green;
                        spawnLocations[i].GetComponent<MeshRenderer>().material = greenMat;
                    }
                }
                
                break;
            case TrainColour.Green:
                mesh.material = greenMat;
                possibleColours.Remove(TrainColour.Green);
                
                //if there is a person of this colour, change them
                for (int i = 0; i < peopleColours.Length; i++)
                {
                    //is this the colour of the train?
                    if (peopleColours[i] == TrainColour.Green)
                    {
                        peopleColours[i] = TrainColour.Blue;
                        spawnLocations[i].GetComponent<MeshRenderer>().material = blueMat;
                    }
                }
                break;
            case TrainColour.Blue:
                mesh.material = blueMat;
                possibleColours.Remove(TrainColour.Blue);
                
                //if there is a person of this colour, change them
                for (int i = 0; i < peopleColours.Length; i++)
                {
                    //is this the colour of the train?
                    if (peopleColours[i] == TrainColour.Blue)
                    {
                        peopleColours[i] = TrainColour.Red;
                        spawnLocations[i].GetComponent<MeshRenderer>().material = redMat;
                    }
                }
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

    private IEnumerator SpawnPerson()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

        //is there room to spawn a person?
        if (peopleWaiting < spawnLocations.Length)
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

        StartCoroutine(SpawnPerson());
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
}
