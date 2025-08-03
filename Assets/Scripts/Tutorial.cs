using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Train train;

    [SerializeField] private Station topStation;
    [SerializeField] private Station bottomStation;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
        train.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            train.enabled = true;
            
            topStation.ActuallySpawnTheGuy();
        }
    }
}
