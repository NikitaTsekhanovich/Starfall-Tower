using GlobalSystems;
using UnityEngine;
using Zenject;

namespace GlobalInstallers
{
    public class SceneDataLoaderInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreenController _loadingScreenController;
        
        public override void InstallBindings()
        {
            var loadingScreenController = Instantiate(_loadingScreenController);
            
            Container
                .Bind<SceneDataLoader>()
                .AsSingle()
                .WithArguments(loadingScreenController)
                .NonLazy();
        }
    }
}
