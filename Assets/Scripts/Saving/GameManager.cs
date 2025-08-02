using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    private int points = 0;

    private void Start()
    {
        pointsText.text = "Points: " + points.ToString();
        StaticObjectHolder.theGameManager = this;
    }

    public void AddPoints(int toAdd)
    {
        points += toAdd;

        pointsText.text = "Points: " + points.ToString();
        
        StaticObjectHolder.theScoreSystem.Score = points;
    }

    public void EndGame()
    {
        StaticObjectHolder.theScoreSystem.SaveScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
