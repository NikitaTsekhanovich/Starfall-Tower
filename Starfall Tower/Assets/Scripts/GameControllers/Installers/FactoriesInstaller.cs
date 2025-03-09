using GameControllers.Entities.Balls;
using GameControllers.Factories;
using GameControllers.Factories.Properties;
using UnityEngine;
using Zenject;

namespace GameControllers.Installers
{
    public class FactoriesInstaller : MonoInstaller
    {
        [SerializeField] private Ball _ballPrefab;
        
        public override void InstallBindings()
        {
            Container
                .Bind<ICanGetBall>()
                .To<BallFactory>()
                .AsSingle()
                .WithArguments(Container, _ballPrefab)
                .NonLazy();
        }
    }
}
