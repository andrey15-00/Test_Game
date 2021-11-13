namespace UnityGame
{
    public class DatabaseTest
    {
        private ControllerRef<DataController> _dataController = new ControllerRef<DataController>();
        public void Test()
        {
            _dataController.Value.SubscribeDataChanged<GameData>(OnDataChanged);

            GameData data_1 = _dataController.Value.GetData<GameData>();
            data_1.Score += 5;
            _dataController.Value.ChangeOrAddData(data_1);
            data_1.Score += 5;
            GameData data_2 = new GameData();
            _dataController.Value.ChangeOrAddData(data_2);
            data_1.Score += 5;
        }

        private void OnDataChanged(GameData data)
        {
            LogWrapper.Log("[DatabaseTest] Data changed! Score: " + data.Score);
        }
    }
}
