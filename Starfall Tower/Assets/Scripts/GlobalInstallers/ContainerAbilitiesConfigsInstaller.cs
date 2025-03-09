using StoreControllers;
using UnityEngine;
using Zenject;

namespace GlobalInstallers
{
    public class ContainerAbilitiesConfigsInstaller : MonoInstaller
    {
        [SerializeField] private ConfigAbility[] _abilitiesConfigs;
        
        public override void InstallBindings()
        {
            Container
                .Bind<ContainerAbilitiesConfigs>()
                .AsSingle()
                .WithArguments(_abilitiesConfigs)
                .NonLazy();
        }
    }
}
