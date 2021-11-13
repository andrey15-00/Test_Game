using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIGameplayScreen : UIBaseScreen
    {
        [SerializeField] private Button _exit;
        [SerializeField] private TMP_Text _score;
        private ControllerRef<GameplayController> _gameplayController = new ControllerRef<GameplayController>();
        private ControllerRef<UIController> _uiController = new ControllerRef<UIController>();
        private ControllerRef<DataController> _dataController = new ControllerRef<DataController>();
        private GameData _gameData;
        private bool _initialized = false;


        private void Init()
        {
            if (_initialized)
            {
                return;
            }

            _exit.onClick.AddListener(OnExitClicked);

            _gameData = _dataController.Value.GetData<GameData>();
            _dataController.Value.SubscribeDataChanged<GameData>(OnDataChanged);

            _initialized = true;
        }


        public override void Show()
        {
            Init();

            _score.text = _gameData.Score.ToString();
            base.Show();
        }

        private async void OnExitClicked()
        {
            await _gameplayController.Value.FinishGame();

            Hide();

            _uiController.Value.GetScreen<UIHomeScreen>().Show();
        }

        private void OnDataChanged(GameData data)
        {
            _score.text = data.Score.ToString();

            data.Score += 10;
        }
    }
}