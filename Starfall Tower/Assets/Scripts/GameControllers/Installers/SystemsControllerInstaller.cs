using GameControllers.Systems;
using Zenject;

namespace GameControllers.Installers
{
    public class SystemsControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<SystemsController>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
