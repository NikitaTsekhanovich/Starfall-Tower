using GameControllers.Camera;
using Zenject;

namespace GameControllers.Installers
{
    public class CameraInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<CameraHandler>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
