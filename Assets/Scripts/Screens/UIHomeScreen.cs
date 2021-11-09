using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGame
{
    public class UIHomeScreen : UIBaseScreen
    {
        [SerializeField] private Button _play;
        [SerializeField] private TMP_Text _score;
        private ControllerRef<GameplayController> _gameplayController = new ControllerRef<GameplayController>();
        private ControllerRef<UIController> _uiController = new ControllerRef<UIController>();
        private ControllerRef<DataController> _dataController = new ControllerRef<DataController>();

        private void Start()
        {
            _play.onClick.AddListener(OnPlayClicked);
        }

        public override void Show()
        {
            GameData gameData = _dataController.Value.GetData<GameData>();
            _score.text = gameData.Score.ToString();
            base.Show();
        }

        private async void OnPlayClicked()
        {
            await _gameplayController.Value.StartGame();

            Hide();

            _uiController.Value.GetScreen<UIGameplayScreen>().Show();
        }
    }
}