namespace UnityGame
{
    public class ControllerRef<T> where T : BaseController
    {
        private T _value;
        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = ControllerStorage.Instance.GetController<T>();
                }

                return _value;
            }
        }
    }
}
