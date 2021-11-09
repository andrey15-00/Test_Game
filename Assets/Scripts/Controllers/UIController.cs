using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame
{
    public class UIController : BaseController
    {
        [SerializeField] private ScreenList _screenList;
        [SerializeField] private RectTransform _screensParent;
        [SerializeField] private Camera _uiCamera;
        private Dictionary<Type, UIBaseScreen> _prefabs = new Dictionary<Type, UIBaseScreen>();
        private Dictionary<Type, UIBaseScreen> _spawnedScreens = new Dictionary<Type, UIBaseScreen>();

        public override void Init()
        {
            foreach(var screen in _screenList.screens)
            {
                Type type = screen.GetType();
                if (!_prefabs.ContainsKey(type))
                {
                    _prefabs[type] = screen;
                }
                else
                {
                    LogWrapper.LogError("[UIController] Found duplicate screen in ScreenList! Type: {0}", type);
                }
            }
        }

        public T GetScreen<T>() where T: UIBaseScreen
        {
            Type type = typeof(T);

            if (_spawnedScreens.ContainsKey(type))
            {
                return _spawnedScreens[type] as T;
            }
            else
            {
                return CreateScreen<T>() as T;
            }
        }

        public void ShowCamera()
        {
            _uiCamera.gameObject.SetActive(true);
        }

        public void HideCamera()
        {
            _uiCamera.gameObject.SetActive(false);
        }

        private UIBaseScreen CreateScreen<T>()
        {
            UIBaseScreen screen;
            Type type = typeof(T);
            if (_prefabs.TryGetValue(type, out screen))
            {
                UIBaseScreen newScreen = Instantiate(screen, _screensParent);
                _spawnedScreens[type] = newScreen;
                return newScreen;
            }

            throw new Exception("[UIController] Screen of type '"+type+"' not found!");
        }
    }
}
