using System.Collections;
using System.Collections.Generic;
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
            if (station.GetColour() == TrainColour.White)
            {
                uncolouredStations.Add(station);
            }
        }
    }

    public void DropOff(Station station, int pointsToAdd)
    {
        gameManager.AddPoints(pointsToAdd);
        
        TrainColour stationTeam = station.GetColour();
        
        station.SetTeam(TrainColour.White);

        int randomIndex = Random.Range(0, uncolouredStations.Count);
        Station currentUncolouredStation = uncolouredStations[randomIndex];
        currentUncolouredStation.SetTeam(stationTeam);

        uncolouredStations.Remove(currentUncolouredStation);
        uncolouredStations.Add(station);

    }
}
