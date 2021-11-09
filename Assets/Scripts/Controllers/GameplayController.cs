using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGame
{
    public class GameplayController : BaseController
    {
        [SerializeField] private string _gameplayScreenName;
        private Player _player;
        private GameplayObjectsMap _objectsMap;
        private bool _running = false;
        private ControllerRef<UIController> _uiController = new ControllerRef<UIController>();
        private ControllerRef<DataController> _dataController = new ControllerRef<DataController>();
        private Coroutine _gameUpdateRoutine;
        private GameData _gameData;

        public async Task StartGame()
        {
            if (_running)
            {
                LogWrapper.LogError("[GameplayController] Already running!");
                return;
            }

            _running = true;

            _uiController.Value.HideCamera();

            var operation = SceneManager.LoadSceneAsync(_gameplayScreenName, LoadSceneMode.Additive);
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            _objectsMap = await GameplayObjectsMapFiller.GetMap();

            _player = Instantiate(_objectsMap.playerPrefab, _objectsMap.playerSpawnParent);
            _player.transform.position = _objectsMap.playerSpawnPoint.position;

            _objectsMap.inputManager.Init(_player);
            _objectsMap.inputManager.Run();

            _gameData = _dataController.Value.GetData<GameData>();

            _gameUpdateRoutine = StartCoroutine(GameLoop());
        }


        public async Task FinishGame()
        {
            if (!_running)
            {
                LogWrapper.LogError("[GameplayController] Not running!");
                return;
            }

            _running = false;

            ResetGame();

            var operation = SceneManager.UnloadSceneAsync(_gameplayScreenName);
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            _uiController.Value.ShowCamera();
        }

        private void ResetGame()
        {
            StopCoroutine(_gameUpdateRoutine);
            _gameUpdateRoutine = null;

            _objectsMap.inputManager.Stop();
            _objectsMap = null;

            Destroy(_player.gameObject);
            _player = null;
        }

        private IEnumerator GameLoop()
        {
            float timePassed = 0f;
            while (_running)
            {
                timePassed += Time.deltaTime;

                _gameData.Score = (int)(timePassed * 10);// "score"

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
