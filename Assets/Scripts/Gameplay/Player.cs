using UnityEngine;

namespace UnityGame
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;

        public void Move(Vector3 input)
        {
            transform.position += input * _moveSpeed * Time.deltaTime;
        }
    }
}
