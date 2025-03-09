using GameControllers.Entities.Balls;
using GameControllers.Factories.Properties;
using UnityEngine;
using Zenject;

namespace GameControllers.Factories
{
    public class BallFactory : ICanGetBall
    {
        private readonly DiContainer _container;
        private readonly Ball _ballPrefab;
        
        public Ball CurrentBall { get; private set; }

        public BallFactory(DiContainer container, Ball ballPrefab)
        {
            _container = container;
            _ballPrefab = ballPrefab;
        }

        public Ball GetBall(Transform transform, Ball ballPrefab = null)
        {
            var currentBallPrefab = ballPrefab;
            
            if (currentBallPrefab == null)
                currentBallPrefab = _ballPrefab;
            
            var newBall = _container.InstantiatePrefabForComponent<Ball>(currentBallPrefab, transform);
            newBall.transform.position = transform.position;
            
            CurrentBall = newBall;

            return newBall;
        }

        public Ball GetCurrentBall() => CurrentBall;
    }
}
