using GameControllers.Logic;
using GlobalSystems;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameControllers.UI
{
    public class FrontCanvasController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _blockNextLevelText;
        [SerializeField] private Button _nextLevelButton;
        
        [Inject] private SceneDataLoader _sceneDataLoader;
        [Inject] private ContainerLevelsConfigs _containerLevelsConfigs;
        [Inject] private GameController _gameController;

        public void ClickPauseGame()
        {
            _gameController.ChangeStateGame(true);
        }

        public void ClickResumeGame()
        {
            _gameController.ChangeStateGame(false);
        }
        
        public void ClickBackToMenu()
        {
            _sceneDataLoader.ChangeScene("MainMenu");
        }

        public void ClickRestartGame()
        {
            _sceneDataLoader.ChangeScene("Game");
        }

        public void ClickNextLevel()
        {
            var currentLevelIndex = _containerLevelsConfigs.CurrentLevel.Index;

            if (currentLevelIndex + 1 < _containerLevelsConfigs.LevelsConfigs.Length)
            {
                _containerLevelsConfigs.SetCurrentLevel(currentLevelIndex + 1);
                _sceneDataLoader.ChangeScene("Game");
            }
            else
            {
                _blockNextLevelText.enabled = true;
                _nextLevelButton.interactable = false;
            }
        }
    }
}
