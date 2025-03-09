using GameControllers.Input;
using GameControllers.Systems;
using Zenject;

namespace GameControllers.Installers
{
    public class InputInstaller : MonoInstaller
    {
        [Inject] private SystemsController _systemsController;
        
        public override void InstallBindings()
        {
            Container
                .Bind<InputController>()
                .FromNew()
                .AsSingle()
                .NonLazy();

            var inputController = Container.Resolve<InputController>();
            _systemsController.RegistrationUpdateSystem(inputController);
        }
    }
}
