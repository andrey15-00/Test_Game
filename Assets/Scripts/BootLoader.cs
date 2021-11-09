using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGame
{

    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private Transform _controllersParent;

        private void Start()
        {
            InitControllers();

            UIController uiController = ControllerStorage.Instance.GetController<UIController>();
            uiController.GetScreen<UIHomeScreen>().Show();
        }

        private void InitControllers()
        {
            ControllerStorage storage = ControllerStorage.Instance;

            for (int a = 0; a < _controllersParent.childCount; ++a)
            {
                BaseController controller = _controllersParent.GetChild(a).GetComponent<BaseController>();
                if (controller != null)
                {
                    storage.AddController(controller.GetType(), controller);
                }
            }

            foreach(var pair in storage.Controllers)
            {
                pair.Value.Init();
            }
        }
    }
}
