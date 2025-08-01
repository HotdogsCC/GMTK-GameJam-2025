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

    private List<GameObject> unoccupiedSpawnLocations = new List<GameObject>();

    [Header("References")] 
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material whiteMat;

    private StationManager stationManager;
    private int people = 0;
    private MeshRenderer mesh;
    

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        stationManager = FindObjectOfType<StationManager>();
        if (stationManager == null)
        {
            Debug.LogWarning("Please add a Station Manager object to the scene");
        }
        SetTeam(myColour);

        foreach (var spawnLocation in spawnLocations)
        {
            unoccupiedSpawnLocations.Add(spawnLocation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //is this the same colour as me?
        Train train = other.GetComponent<Train>();
        if (train.GetColour() == myColour)
        {
            stationManager.DropOff(this, (train.GetPeople()));
            train.TakePeople();
        }
    }

    public void SetTeam(TrainColour col)
    {
        myColour = col;
        
        switch (myColour)
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

        if (unoccupiedSpawnLocations.Count != 0)
        {
            
        }
    }
}
