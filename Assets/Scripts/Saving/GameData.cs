using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    /// <summary>
    /// contains a name and a score and thats it.
    /// </summary>
    [System.Serializable]
    public class NameScorePair
    {
        public string name;
        public int score;

        public NameScorePair(string _name, int _score)
        {
            name = _name;
            score = _score;
        }
    }

    /// <summary>
    /// used for saving;
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        //these are where the high scores are stored
        public List<NameScorePair> highScores;

        //the class initializer sets highScores to a new list of highScores
        public GameData()
        {
            highScores = new List<NameScorePair>();
        }

        /// <summary>
        /// returns the index of highscores at which the name, "name" appears;
        /// </summary>
        public int IndexOfName(string name)
        {
            for (int i = 0; i < highScores.Count; i++)
            {
                if (highScores[i].name == name)
                    return i;
            }
            if (highScores.Count == 0)
                return 1;
            return highScores.Count;
        }

        /// <summary>
        /// bubble sorts the highScores by each score
        /// </summary>
        void SortHighScores()
        {
            List<NameScorePair> highScoresCopy = highScores;
            bool isSorted = false;
            while (!isSorted)
            {
                bool possiblySorted = true;
                for (int j = 0; j < highScoresCopy.Count - 1; j++)
                {
                    if (highScoresCopy[j].score < highScoresCopy[j + 1].score)
                    {
                        possiblySorted = false;
                        NameScorePair higher = highScoresCopy[j];
                        NameScorePair lower = highScoresCopy[j + 1];
                        highScoresCopy[j] = lower;
                        highScoresCopy[j + 1] = higher;
                    }
                }
                isSorted = possiblySorted;
            }
            highScores = highScoresCopy;
        }

        /// <summary>
        /// reduces the number of highscores down to 8 by removing the last members of the list.
        /// </summary>
        void TrimHighScores()
        {
            int maxHighScores = 8;
            if (highScores.Count > maxHighScores)
            {
                for (int i = highScores.Count; i > maxHighScores; i--)
                    highScores.RemoveAt(i - 1);
            }
        }

        /// <summary>
        /// Adds a highScore with name "name" and score "score" to the list of highscores and sorts it and trims it if the number of highscores excedes 8
        /// </summary>
        public void AddScore(string name, int score)
        {
            int indexOfName = IndexOfName(name);
            if (indexOfName < highScores.Count)
            {
                if (highScores[indexOfName].score < score)
                {
                    highScores[indexOfName] = new NameScorePair(name, score);
                }
            }
            else
            {
                highScores.Add(new NameScorePair(name, score));
            }
            SortHighScores();
            TrimHighScores();
        }
    }
}