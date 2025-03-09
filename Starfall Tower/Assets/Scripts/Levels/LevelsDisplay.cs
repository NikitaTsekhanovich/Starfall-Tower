using System.Collections.Generic;
using GlobalSystems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Levels
{
    public class LevelsDisplay : MonoBehaviour
    {
        [SerializeField] private Transform _levelsContainer;
        [SerializeField] private Transform _dotsContainer;
        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private GameObject _levelsGridPrefab;
        [SerializeField] private Button _previousGridButton; 
        [SerializeField] private Button _nextGridButton;
        [SerializeField] private AudioSource _clickLevelSound;
        
        [Inject] private ContainerLevelsConfigs _containerLevelsConfigs;
        [Inject] private SceneDataLoader _sceneDataLoader;

        private const int MaximumLevelItemsOnGrid = 4;
        private readonly List<GameObject> _levelsGrids = new();
        private readonly List<GameObject> _dots = new();
        private int _currentIndexLevelGrid;

        private void Start()
        {
            LoadLevelsItems();
        }

        private void LoadLevelsItems()
        {
            var index = 0;
            var levelGrid = Instantiate(_levelsGridPrefab, _levelsContainer);
            _levelsGrids.Add(levelGrid);
            
            var dot = Instantiate(_dotPrefab, _dotsContainer);
            _dots.Add(dot);
            
            foreach (var levelConfig in _containerLevelsConfigs.LevelsConfigs)
            {
                if (index % MaximumLevelItemsOnGrid == 0 && index != 0)
                {
                    levelGrid = Instantiate(_levelsGridPrefab, _levelsContainer);
                    levelGrid.gameObject.SetActive(false);
                    _levelsGrids.Add(levelGrid);
            
                    dot = Instantiate(_dotPrefab, _dotsContainer);
                    _dots.Add(dot);
                    
                    _nextGridButton.interactable = true;
                }
                
                var newLevelItem = Instantiate(levelConfig.LevelItem, levelGrid.transform);
                newLevelItem.Init(
                    levelConfig,
                    ChooseLevel);
            
                index++;
            }
        }

        private void ChooseLevel(int indexLevel)
        {
            _containerLevelsConfigs.SetCurrentLevel(indexLevel);
            _sceneDataLoader.ChangeScene("Game");
            _clickLevelSound.Play();
        }

        public void ClickNextLevelGrid()
        {
            if (_currentIndexLevelGrid + 1 < _levelsGrids.Count)
            {
                _previousGridButton.interactable = true;
                
                _levelsGrids[_currentIndexLevelGrid].SetActive(false);
                _currentIndexLevelGrid++;
                _levelsGrids[_currentIndexLevelGrid].SetActive(true);
            }
            
            if (_currentIndexLevelGrid >= _levelsGrids.Count - 1)
            {
                _nextGridButton.interactable = false;
            }
        }
        
        public void ClickPreviousLevelGrid()
        {
            if (_currentIndexLevelGrid - 1 >= 0)
            {
                _nextGridButton.interactable = true;
                
                _levelsGrids[_currentIndexLevelGrid].SetActive(false);
                _currentIndexLevelGrid--;
                _levelsGrids[_currentIndexLevelGrid].SetActive(true);
            }
            
            if (_currentIndexLevelGrid <= 0)
            {
                _previousGridButton.interactable = false;
            }
        }

        public void ClickStartArcade()
        {
            _containerLevelsConfigs.CreateLevelsQueue();
            _sceneDataLoader.ChangeScene("Game", ModeGame.Arcade);
        }
    }
}
