using UnityEngine;

namespace Core.Models
{
    public class GameState
    {
        public int Score;
        public int Streak;
        public float CurrentTime;
        public int HighScore;
        public float BestTime;
        public byte CurrentLifes;

        public void Serialize()
        {
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.SetFloat("BestTime", BestTime);
        }
        public void Deserialize()
        {
            HighScore = PlayerPrefs.GetInt("HighScore", HighScore);
            BestTime = PlayerPrefs.GetFloat("BestTime", BestTime);
        }
    }
}
