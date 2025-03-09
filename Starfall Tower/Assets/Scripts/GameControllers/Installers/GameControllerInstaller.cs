using GameControllers.Logic;
using GameControllers.Tower;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameControllers.Installers
{
    public class GameControllerInstaller : MonoInstaller
    {
        [SerializeField] private TMP_Text _ballsCountText;
        [SerializeField] private Image _levelProgressBar;
        [SerializeField] private GameObject _winBlock;
        [SerializeField] private GameObject _loseBlock;
        [SerializeField] private TMP_Text _delayBeforeLoseText;
        
        [Inject] private ContainerLevelsConfigs _containerLevelsConfigs;
        
        public override void InstallBindings()
        {
            Container.Bind<GameController>()
                .AsCached()
                .WithArguments(
                    _ballsCountText, 
                    _levelProgressBar, 
                    _containerLevelsConfigs.CurrentLevel,
                    _winBlock,
                    _loseBlock,
                    _delayBeforeLoseText)
                .NonLazy();
        }
    }
}
