using System.Threading.Tasks;
using UnityEngine;

namespace UnityGame
{
    public class InputManager : MonoBehaviour
    {
        private Player _player;
        private bool _running;

        public void Init(Player player)
        {
            _player = player;
        }

        public void Run()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        private void Update()
        {
            if (!_running)
            {
                return;
            }

            float moveX = Input.GetAxis("Horizontal");

            _player.Move(new Vector3(moveX, 0, 0));
        }
    }
}
