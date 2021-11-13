using System;

namespace UnityGame
{
    public class GameData : IData
    {
        [NonSerialized] private int _score;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                Changed?.Invoke();
            }
        }

        public event Action Changed;

        public IData Clone()
        {
            return (GameData)MemberwiseClone();
        }

        public void Reset()
        {
            _score = 0;
        }
    }
}
