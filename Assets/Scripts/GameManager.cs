using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    private int points = 0;

    private void Start()
    {
        pointsText.text = "Points: " + points.ToString();
    }

    public void AddPoints(int toAdd)
    {
        points += toAdd;

        pointsText.text = "Points: " + points.ToString();
    }
}
