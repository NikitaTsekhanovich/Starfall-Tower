using DG.Tweening;
using GameControllers.Camera;
using GameControllers.Factories.Properties;
using GameControllers.Input;
using GameControllers.Logic;
using UnityEngine;
using Zenject;

namespace GameControllers.Entities.Balls
{
    public class BallSpawner : MonoBehaviour
    {
        [Inject] private CameraHandler _cameraHandler;
        [Inject] private ICanGetBall _factoryBall;
        [Inject] private InputController _inputController;
        [Inject] private GameController _gameController;

        private bool _isSpawning;
        private const float DelaySpawn = 0.5f;

        private void Start()
        {
            SubscribeEvents();
            SpawnFirstBall();
        }
        
        private void SubscribeEvents()
        {
            _inputController.OnChooseTargetThrow += SpawnBall;
        }
        
        private void UnsubscribeEvents()
        {
            _inputController.OnChooseTargetThrow -= SpawnBall;
        }

        private void SpawnFirstBall()
        {
            var newBall = _factoryBall.GetBall(_cameraHandler.SpawnBallTransform);
            newBall.transform.localScale = new Vector3(3f, 3f, 3f);
            newBall.AlreadyThrow();
        }

        private void SpawnBall()
        {
            if (_isSpawning) return;
            
            _gameController.DecreaseBalls();
            if (_gameController.BallNumberController.CurrentCountBalls <= 0) return;
            
            _isSpawning = true;
            var newBall = _factoryBall.GetBall(_cameraHandler.SpawnBallTransform);
            
            DOTween.Sequence()
                .Append(newBall.transform.DOScale(new Vector3(3f, 3f, 3f), DelaySpawn))
                .AppendCallback(() =>
                {
                    newBall.AlreadyThrow();
                    _isSpawning = false;
                });
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
    }
}
