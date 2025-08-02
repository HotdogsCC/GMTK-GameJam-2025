using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class HighScoreListItem : MonoBehaviour
{
    [Header("-- HighScore Item Display Components --")]
    [SerializeField, Tooltip("The text that displays what position a score is in")]
    private TextMeshProUGUI positionDisplay;
    [SerializeField, Tooltip("The text that displays what the name of the highscoere is attributed to")]
    private TextMeshProUGUI nameDisplay;
    [SerializeField, Tooltip("The text that displays the actual hiscore score")]
    private TextMeshProUGUI scoreDisplay;

    /// <summary>
    /// set the values of this particular highscore listitem
    /// </summary>
    public void SetValues(int position, string name, int score)
    {
        positionDisplay.text = position.ToString();
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();
    }
}
