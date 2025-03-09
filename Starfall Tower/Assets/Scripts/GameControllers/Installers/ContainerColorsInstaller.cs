using GameControllers.Entities;
using Zenject;

namespace GameControllers.Installers
{
    public class ContainerColorsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ContainerColors>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
