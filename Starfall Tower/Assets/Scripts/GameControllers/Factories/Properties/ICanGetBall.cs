using GameControllers.Entities.Balls;
using UnityEngine;

namespace GameControllers.Factories.Properties
{
    public interface ICanGetBall
    {
        public Ball GetBall(Transform transform, Ball ballPrefab = null);
        public Ball GetCurrentBall();
    }
}
