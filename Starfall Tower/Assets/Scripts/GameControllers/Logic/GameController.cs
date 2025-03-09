using DG.Tweening;
using GameControllers.Logic.Components;
using GameControllers.Tower;
using GlobalSystems;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameControllers.Logic
{
    public class GameController
    {  
        [Inject] private ContainerLevelsConfigs _containerLevelsConfigs;
        [Inject] private SceneDataLoader _sceneDataLoader;
        
        private readonly LevelProgressController _levelProgressController;
        private readonly GameObject _winBlock;
        private readonly GameObject _loseBlock;
        private readonly TMP_Text _delayBeforeLoseText;
        private const int DelayLose = 5;
        private Sequence _delayBeforeLose;
        private bool _isWin;
        private bool _isLose;

        public BallNumberController BallNumberController { get; private set; }
        public bool GameIsPaused { get; private set; }
        public bool IsChooseAbility { get; private set; }
        
        public GameController(
            TMP_Text ballsCountText,
            Image levelProgressBar,
            LevelConfig levelConfig,
            GameObject winBlock,
            GameObject loseBlock,
            TMP_Text delayBeforeLoseText)
        {
            BallNumberController = 
                new BallNumberController(
                    ballsCountText, 
                    DelayBeforeLose, 
                    levelConfig.BallCount);
            _levelProgressController = 
                new LevelProgressController(
                    levelProgressBar, 
                    SetWin, 
                    levelConfig.PatternSpawn.BlocksHeight - TowerSpawner.IndexVictoryBorderRow);
            _winBlock = winBlock;
            _loseBlock = loseBlock;
            _delayBeforeLoseText = delayBeforeLoseText;
        }

        public void ChooseAbility(bool isChooseAbility)
        {
            IsChooseAbility = isChooseAbility;
        }

        public void DecreaseBalls()
        {
            BallNumberController.DecreaseBalls();
        }

        public void DecreaseRow()
        {
            _levelProgressController.DecreaseRow();
        }

        public void ChangeStateGame(bool gameIsPaused)
        {
            GameIsPaused = gameIsPaused;
        }
        
        private void SetWin()
        {
            if (_isLose) return;
            
            _isWin = true;
            _delayBeforeLose.Kill();
            _delayBeforeLoseText.gameObject.SetActive(false);
            
            GameIsPaused = true;
            _winBlock.SetActive(true);

            if (_sceneDataLoader.ModeGame == ModeGame.Levels)
            {
                var currentLevelIndex = _containerLevelsConfigs.CurrentLevel.Index;

                if (currentLevelIndex + 1 < _containerLevelsConfigs.LevelsConfigs.Length)
                {
                    PlayerPrefs.SetInt(
                        _containerLevelsConfigs.LevelsConfigs[currentLevelIndex + 1].StateLevelKey, 
                        (int)TypeStateLevel.IsOpen);
                }
            }
            else if (_sceneDataLoader.ModeGame == ModeGame.Arcade)
            {
                _containerLevelsConfigs.ChooseNextRandomLevel();
            }
        }

        private void SetLose()
        {
            if (_isWin) return;
                
            _isLose = true;
            _loseBlock.SetActive(true);
        }

        private void DelayBeforeLose()
        {
            GameIsPaused = true;
            var currentDelay = DelayLose;

            _delayBeforeLose = DOTween.Sequence()
                .AppendCallback(() => _delayBeforeLoseText.text = currentDelay.ToString())
                .Append(_delayBeforeLoseText.transform.DOScale(Vector3.one, 0.5f))
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    _delayBeforeLoseText.transform.localScale = Vector3.zero;
                    currentDelay--;
                })
                .SetLoops(5, LoopType.Restart)
                .OnKill(SetLose);
        }
    }
}
