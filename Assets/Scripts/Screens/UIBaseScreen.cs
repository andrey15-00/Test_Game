using UnityEngine;

namespace UnityGame
{
    public class UIBaseScreen : MonoBehaviour
    {
        [SerializeField] protected GameObject _root;

        public virtual void Show()
        {
            _root.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public virtual void Hide()
        {
            _root.gameObject.SetActive(false);
        }
    }
}