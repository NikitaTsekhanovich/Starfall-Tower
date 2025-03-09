using System;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Image _previewLevelImage;
        [SerializeField] private Image _levelNumberImage;
        [SerializeField] private GameObject _openLevelBlock;
        [SerializeField] private GameObject _closeLevelBlock;
        [SerializeField] private Button _playButton;
        
        private int _indexLevel;
        private Action<int> _chooseLevelAction;
        private LevelConfig _levelConfig;

        public void Init(
            LevelConfig levelConfig,
            Action<int> chooseLevelAction)
        {
            _previewLevelImage.sprite = levelConfig.PreviewLevelSprite;
            _indexLevel = levelConfig.Index;
            _levelNumberImage.sprite = levelConfig.LevelNumberSprite;
            _chooseLevelAction = chooseLevelAction;
            _levelConfig = levelConfig;

            CheckStateLevel();
        }

        private void CheckStateLevel()
        {
            _openLevelBlock.SetActive(false);
            _closeLevelBlock.SetActive(false);
            
            if (_levelConfig.StateLevel == TypeStateLevel.IsClosed)
            {
                _closeLevelBlock.SetActive(true);
                _playButton.interactable = false;
            }
            else if (_levelConfig.StateLevel == TypeStateLevel.IsOpen)
            {
                _openLevelBlock.SetActive(true);
                _playButton.interactable = true;
            }
        }

        public void ClickPlay()
        {
            _chooseLevelAction.Invoke(_indexLevel);
            CheckStateLevel();
        }
    }
}
