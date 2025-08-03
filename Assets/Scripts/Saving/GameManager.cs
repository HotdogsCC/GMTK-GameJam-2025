using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int pointsNeededForDoubleSpeed = 50;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject canvas;
    [SerializeField] private List<GameObject> tutorialElements = new List<GameObject>();
    private bool controlsAreDisplayed = true;
    private int points = 0;
    private MusicPlayer musicPlayer;

    private float currentTimeScale = 1.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlsAreDisplayed = !controlsAreDisplayed;
            foreach(GameObject tutorialElement in tutorialElements)
            {
                tutorialElement.SetActive(controlsAreDisplayed);
            }
            //canvas.SetActive(controlsAreDisplayed);
        }

        //is game paused?
        if (Time.timeScale == 0.0f)
        {
            return;
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
        currentTimeScale = Time.timeScale;
        
        musicPlayer = FindObjectOfType<MusicPlayer>();
        if (musicPlayer == null)
        {
            Debug.LogWarning("Please add the Music Player into the scene");
        }
        pointsText.text = "Points: " + points.ToString();
        
        StaticObjectHolder.theGameManager = this;
    }

    public void AddPoints(int toAdd)
    {
        points += toAdd;

        pointsText.text = "Points: " + points.ToString();
        
        musicPlayer.UpdateSpeed(points);
        
        float speed = (float)points / (float)pointsNeededForDoubleSpeed + 1.0f;

        currentTimeScale = speed;
        
        StaticObjectHolder.theScoreSystem.Score = points;
    }

    public int GetPoints()
    {
        return points;
    }
    
    public void EndGame()
    {
        StaticObjectHolder.theScoreSystem.SaveScore();
        GameOver.finalScore = points;
        GameOver.previousSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        
        SceneManager.LoadScene("GameOver");
    }
}
