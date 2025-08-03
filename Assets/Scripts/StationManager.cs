using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StationManager : MonoBehaviour
{
    [Header("Red Train Requirement")] [SerializeField]
    private int pointsNeededForRedTrain = 100;
    
    [Header("Available Colours")] 
    [SerializeField] public bool isUsingRed = true;
    [SerializeField] public bool isUsingGreen = true;
    [SerializeField] public bool isUsingBlue = true;

    [Header("Train Prefab Reference")] [SerializeField]
    private GameObject trainPrefab;
    
    [Header("Particles")] 
    [SerializeField] private GameObject confettiParticles;
    [SerializeField] private GameObject regularParticles;
    
    private GameManager gameManager;
    private Station[] stations;
    private List<Station> uncolouredStations = new List<Station>();
    private List<Station> inactiveStations = new List<Station>();
    
    private List<Transform> redTrainTransforms = new List<Transform>();

    private Train[] trains;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] redTrainSpawnObjects = GameObject.FindGameObjectsWithTag("RedTrainSpawnLocation");

        foreach (var redTrain in redTrainSpawnObjects)
        {
            Transform tempTransform = redTrain.transform;
            redTrainTransforms.Add(tempTransform);
            redTrain.SetActive(false);
        }
    
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Please add a Game Manager to the scene");
        }
    
        stations = FindObjectsOfType<Station>();
        trains = FindObjectsOfType<Train>();

        foreach (var station in stations)
        {
            // If a station needs points to activate, it's inactive.
            if (station.GetPointsNeeded() != 0)
            {
                inactiveStations.Add(station);
                station.GetComponent<MeshRenderer>().enabled = false;
                station.GetComponent<BoxCollider>().enabled = false;
                station.neighbouringBuildings.SetActive(false);
            }
            // An active station can be uncoloured.
            else if (station.GetColour() == TrainColour.White)
            {
                uncolouredStations.Add(station);
            }
        }
    }

    public void DropOff(Station station, int pointsToAdd)
    {
        Instantiate(confettiParticles, station.transform.position, station.transform.rotation);
        
        //add the points
        gameManager.AddPoints(pointsToAdd);
        
        //see if any stations can be activated
        foreach (var inactiveStation in inactiveStations.ToList())
        {
            if (inactiveStation.GetPointsNeeded() <= gameManager.GetPoints())
            {
                inactiveStation.GetComponent<MeshRenderer>().enabled = true;
                inactiveStation.GetComponent<BoxCollider>().enabled = true;
                inactiveStation.neighbouringBuildings.SetActive(true);
                Instantiate(regularParticles, inactiveStation.transform.position, inactiveStation.transform.rotation);
                uncolouredStations.Add(inactiveStation);
                inactiveStation.StartCoroutine(inactiveStation.SpawnPerson());
                inactiveStations.Remove(inactiveStation);
            }
        }
        
        
        TrainColour stationTeam = station.GetColour();
        
        station.SetTeam(TrainColour.White);

        int randomIndex = Random.Range(0, uncolouredStations.Count);
        Station currentUncolouredStation = uncolouredStations[randomIndex];
        currentUncolouredStation.SetTeam(stationTeam);

        uncolouredStations.Remove(currentUncolouredStation);
        uncolouredStations.Add(station);
        
        //see if the red train can be added
        if (!isUsingRed && gameManager.GetPoints() > pointsNeededForRedTrain)
        {
            SpawnRedTrain();
        }

    }
    

    public void SpawnRedTrain()
    {
        isUsingRed = true;

        //find the furthest away spot to spawn
        float furthestDistance = 
            Vector3.Distance(redTrainTransforms[0].position, trains[0].transform.position)
            + Vector3.Distance(redTrainTransforms[0].position, trains[1].transform.position);
        Transform furthestSpawn = redTrainTransforms[0];
        foreach (Transform possibleSpawn in redTrainTransforms)
        {
            float thisDistance = 
                Vector3.Distance(possibleSpawn.position, trains[0].transform.position)
                + Vector3.Distance(possibleSpawn.position, trains[1].transform.position);

            if (thisDistance > furthestDistance)
            {
                furthestDistance = thisDistance;
                furthestSpawn = possibleSpawn;
            }
        }

        Instantiate(trainPrefab, furthestSpawn.position, furthestSpawn.rotation);

        foreach (var stat in stations)
        {
            //tell the other stations they can start spawning red
            stat.AddRed();
        }
        
        //turn an uncoloured station into a red one
        int randomIndex = Random.Range(0, uncolouredStations.Count);
        Station currentUncolouredStation = uncolouredStations[randomIndex];
        currentUncolouredStation.SetTeam(TrainColour.Red);

        uncolouredStations.Remove(currentUncolouredStation);
    }
}
