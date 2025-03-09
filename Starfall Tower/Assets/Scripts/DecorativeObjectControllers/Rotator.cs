using UnityEngine;

namespace DecorativeObjectControllers
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _speedRotate;
        
        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _speedRotate);
        }
    }
}
