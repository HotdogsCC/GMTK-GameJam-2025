using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTExt;
    public static int finalScore = 0;
    public static int previousSceneBuildIndex;

    public void ReturnToTitle()
    {
        finalScore = 0;
        SceneManager.LoadScene(0);
    }

    public void TryAgain()
    {
        finalScore = 0;
        SceneManager.LoadScene(previousSceneBuildIndex);
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        scoreTExt.text = finalScore.ToString();
    }
}
