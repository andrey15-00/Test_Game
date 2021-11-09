using System;

namespace UnityGame
{
    public class GameData : IDatabaseData
    {
        [NonSerialized] private int _score;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                ScoreChanged?.Invoke(_score);
            }
        }

        public event Action<int> ScoreChanged;

        public void Reset()
        {
            _score = 0;
        }
    }
}
