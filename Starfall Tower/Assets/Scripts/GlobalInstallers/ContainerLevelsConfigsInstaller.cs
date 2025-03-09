using Levels;
using UnityEngine;
using Zenject;

namespace GlobalInstallers
{
    public class ContainerLevelsConfigsInstaller : MonoInstaller
    {
        [SerializeField] private LevelConfig[] _levelConfigs;
        
        public override void InstallBindings()
        {
            Container
                .Bind<ContainerLevelsConfigs>()
                .AsSingle()
                .WithArguments(_levelConfigs)
                .NonLazy();
        }
    }
}
