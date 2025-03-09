using GameControllers.Entities.Blocks;
using Zenject;

namespace GameControllers.Installers
{
    public class ContainerPrefabsBlocksInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ContainerPrefabsBlocks>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
