using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    [Header("Available Colours")] 
    [SerializeField] public bool isUsingRed = true;
    [SerializeField] public bool isUsingGreen = true;
    [SerializeField] public bool isUsingBlue = true;
    
    private GameManager gameManager;
    private Station[] stations;
    private List<Station> uncolouredStations = new List<Station>();
    private List<Station> inactiveStations = new List<Station>();
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Please add a Game Manager to the scene");
        }
        
        stations = FindObjectsOfType<Station>();

        foreach (var station in stations)
        {
            if (station.GetPointsNeeded() != 0)
            {
                inactiveStations.Add(station);
                station.GetComponent<MeshRenderer>().enabled = false;
                station.GetComponent<BoxCollider>().enabled = false;
            }
            
            if (station.GetColour() == TrainColour.White)
            {
                uncolouredStations.Add(station);
            }
        }
    }

    public void DropOff(Station station, int pointsToAdd)
    {
        //add the points
        gameManager.AddPoints(pointsToAdd);
        
        //see if any stations can be activated
        foreach (var inactiveStation in inactiveStations.ToList())
        {
            if (inactiveStation.GetPointsNeeded() <= gameManager.GetPoints())
            {
                inactiveStation.GetComponent<MeshRenderer>().enabled = true;
                inactiveStation.GetComponent<BoxCollider>().enabled = true;
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

    }
}
