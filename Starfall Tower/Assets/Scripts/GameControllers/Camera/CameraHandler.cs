using DG.Tweening;
using GameControllers.Input;
using GameControllers.Tower;
using GlobalSystems;
using UnityEngine;
using Zenject;

namespace GameControllers.Camera
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform _targetRotate;
        
        private Sequence _lowerAnimation;
        private float _angle;
        private float _blockScaleY;
        private float _currentPositionY;
        private const float StartPositionY = 5f;
        private const float RotationSpeed = 500f;
        private const float TimeLowerAnimation = 1f;
        private const float BaseFieldOfView = 60f;
        private UnityEngine.Camera _camera;
        
        [Inject] private InputController _inputController;
        
        [field: SerializeField] public Transform SpawnBallTransform { get; private set; }

        public void SetStartHeight(float blockScaleY, int blocksHeight)
        {
            _camera = UnityEngine.Camera.main;
            _blockScaleY = blockScaleY;
            _currentPositionY = _blockScaleY * blocksHeight + StartPositionY;
            _targetRotate.transform.position = new Vector3(0f, _currentPositionY, 0f);
            
            transform.position = new Vector3(
                transform.position.x, 
                _currentPositionY, 
                transform.position.z);

            StartAnimation();
        }

        private void StartAnimation()
        {
            var currentValue = _camera.fieldOfView;
            
            DOTween.Sequence()
                .AppendInterval(LoadingScreenController.DelayAnimation)
                .AppendCallback(() =>
                {
                    DOTween.To(() => currentValue, x => currentValue = x, BaseFieldOfView, 4f)
                        .OnUpdate(() =>
                        {
                            _camera.fieldOfView = currentValue;
                        });
                });
        }
        
        private void OnEnable()
        {
            RowCollisionHandler.OnRowCleared += LowerCamera;
        }

        private void OnDestroy()
        {
            RowCollisionHandler.OnRowCleared -= LowerCamera;
            transform.DOKill();
            _targetRotate.DOKill();
            _lowerAnimation.Kill();
        }

        public void Rotate()
        {
            var horizontal = UnityEngine.Input.GetAxis("Mouse X");

            // ПРОВЕРИТЬ С ТЕЛЕФОНА С НЕСКОЛЬКИМИ ТАЧАМИ.
            // if (UnityEngine.Input.touchCount > 0)
            // {
            //     var touch = UnityEngine.Input.GetTouch(0);
            //
            //     if (touch.phase == TouchPhase.Moved)
            //     {
            //         horizontal = touch.deltaPosition.x;
            //     }
            // }
            // else if (UnityEngine.Input.GetMouseButton(0))
            // {
            //     horizontal = UnityEngine.Input.GetAxis("Mouse X");
            // }

            transform.RotateAround(
                _targetRotate.position,
                Vector3.up,
                horizontal * RotationSpeed * Time.deltaTime);
            
            transform.LookAt(_targetRotate.position);
        }

        public void Shake()
        {
            transform.DOShakePosition(1f);
        }
        
        private void LowerCamera()
        {
            _currentPositionY -= _blockScaleY;

            if (_lowerAnimation != null && _lowerAnimation.active)
            {
                _lowerAnimation.Kill();
            }
            
            _lowerAnimation = DOTween.Sequence()
                .Append(transform.DOMoveY(
                    _currentPositionY,
                    TimeLowerAnimation));

            _targetRotate.transform.DOMoveY(
                _currentPositionY,
                TimeLowerAnimation);
        }
    }
}
