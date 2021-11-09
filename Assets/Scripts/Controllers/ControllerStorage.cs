using System;
using System.Collections.Generic;

namespace UnityGame
{
    public class ControllerStorage
    {
        private static ControllerStorage _instance = new ControllerStorage();
        private Dictionary<Type, BaseController> _controllers = new Dictionary<Type, BaseController>();

        public static ControllerStorage Instance => _instance;
        public Dictionary<Type, BaseController> Controllers => _controllers;


        public void AddController(Type type, BaseController controller)
        {
            _controllers.Add(type, controller);
        }

        public T GetController<T>() where T : BaseController
        {
            Type type = typeof(T);
            if (_controllers.ContainsKey(type))
            {
                return _controllers[type] as T;
            }

            return null;
        }
    }
}
