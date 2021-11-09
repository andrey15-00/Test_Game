using System.Threading.Tasks;
using UnityEngine;

namespace UnityGame
{
    public class GameplayObjectsMap
    {
        public Transform playerSpawnPoint;
        public Transform playerSpawnParent;
        public Player playerPrefab;
        public InputManager inputManager;
        public Camera camera;
    }

    public class GameplayObjectsMapFiller : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _playerSpawnParent;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private Camera _camera;
        private static GameplayObjectsMap _map;

        private void Awake()
        {
            _map = new GameplayObjectsMap()
            {
                playerSpawnPoint = _playerSpawnPoint,
                playerSpawnParent = _playerSpawnParent,
                playerPrefab = _playerPrefab,
                inputManager = _inputManager,
                camera = _camera,
            };
        }

        public static async Task<GameplayObjectsMap> GetMap()
        {
            while (_map == null)
            {
                await Task.Yield();
            }

            return _map;
        }
    }
}
