using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int pointsNeededForDoubleSpeed = 50;
    [SerializeField] private TextMeshProUGUI pointsText;
    private int points = 0;
    private MusicPlayer musicPlayer;

    private float currentTimeScale = 1.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPoints(1);
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = currentTimeScale * 2.0f;
        }
        
        else
        {
            Time.timeScale = currentTimeScale;
        }
    }

    private void Start()
    {
        Time.timeScale = currentTimeScale;
        
        musicPlayer = FindObjectOfType<MusicPlayer>();
        if (musicPlayer == null)
        {
            Debug.LogWarning("Please add the Music Player into the scene");
        }
        pointsText.text = "Points: " + points.ToString();
    }

    public void AddPoints(int toAdd)
    {
        points += toAdd;

        pointsText.text = "Points: " + points.ToString();
        
        musicPlayer.UpdateSpeed(points);
        
        float speed = (float)points / (float)pointsNeededForDoubleSpeed + 1.0f;

        currentTimeScale = speed;
    }
}
