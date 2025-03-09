using System;
using GameControllers.Camera;
using GameControllers.Logic;
using GameControllers.Systems.Properties;
using UnityEngine;
using Zenject;

namespace GameControllers.Input
{
    public class InputController : IHaveUpdate
    {
        private float _previousClickX;
        private readonly UnityEngine.Camera _camera;
        
        [Inject] private CameraHandler _cameraHandler;
        [Inject] private GameController _gameController;
        
        public bool IsThrowMeteors { get; private set; }
        public Vector3 TargetClick { get; private set; }
        public bool IsMoveCamera { get; private set; }
        public event Action OnChooseTargetThrow;
        public event Action OnChooseTargetMeteor;

        public InputController()
        {
            _camera = UnityEngine.Camera.main;
        }
        
        public void Update()
        {
            if (_gameController.GameIsPaused || _gameController.IsChooseAbility) return;
            
            CheckClickInput();
        }

        public void SetAiming(bool isAiming)
        {
            IsThrowMeteors = isAiming;
        }

        private void CheckClickInput()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                IsMoveCamera = false;
                
                var clickMousePosition = UnityEngine.Input.mousePosition;
                _previousClickX = clickMousePosition.x;
            }

            if (UnityEngine.Input.GetMouseButton(0))
            {
                var clickMousePosition = UnityEngine.Input.mousePosition;
                
                if (Math.Abs(clickMousePosition.x - _previousClickX) >= 0.03f || IsMoveCamera)
                {
                    _cameraHandler.Rotate();
                    IsMoveCamera = true;
                    _previousClickX = clickMousePosition.x;
                }
            }
            
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (IsMoveCamera)
                {
                    IsMoveCamera = false;
                    return;
                }
                
                var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                var hits = Physics.RaycastAll(ray);

                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("GameEntity"))
                    {
                        TargetClick = ray.GetPoint(hit.distance);
                        
                        if (!IsThrowMeteors)
                            OnChooseTargetThrow?.Invoke();
                        else
                            OnChooseTargetMeteor?.Invoke();
                        
                        return;
                    }
                }
            }
        }
    }
}

