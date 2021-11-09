using UnityEngine;

namespace UnityGame
{
    [CreateAssetMenu(fileName = "ScreenList", menuName = "Data/ScreenList", order = 1)]
    public class ScreenList : ScriptableObject
    {
        public UIBaseScreen[] screens;
    }
}