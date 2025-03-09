using GameControllers.Camera;
using GameControllers.Entities.Balls;
using GameControllers.Factories.Properties;
using UnityEngine;
using Zenject;

namespace GameControllers.Abilities.Activators
{
    public class PlanetActivator : AbilityActivator
    {
        [SerializeField] private Ball _planetPrefab;
        
        [Inject] private ICanGetBall _ballFactory;
        [Inject] private CameraHandler _cameraHandler;
        
        protected override void UseAbility()
        {
            var currentBall = _ballFactory.GetCurrentBall();
            currentBall.DoDestroy();

            _ballFactory.GetBall(_cameraHandler.SpawnBallTransform, _planetPrefab);
        }
    }
}
