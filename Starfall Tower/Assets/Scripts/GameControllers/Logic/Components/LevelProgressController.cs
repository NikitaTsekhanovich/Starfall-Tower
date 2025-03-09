using System;
using DG.Tweening;
using UnityEngine.UI;

namespace GameControllers.Logic.Components
{
    public class LevelProgressController
    {
        private readonly Image _levelProgressBar;
        private readonly Action _onWin;
        private readonly int _countRows;
        private const float DelayUpdateProgressBar = 1f;
        private int _currentDestroyedRows;
        private float _currentProgress;
        
        public LevelProgressController(Image levelProgressBar, Action onWin, int countRows)
        {
            _levelProgressBar = levelProgressBar;
            _onWin = onWin;
            _countRows = countRows;
        }

        public void DecreaseRow()
        {
            _currentDestroyedRows++;
            _currentProgress = _currentDestroyedRows / (float)_countRows;
            
            UpdateProgressBar();
            
            if (_currentDestroyedRows >= _countRows)
            {
                _onWin.Invoke();
                _levelProgressBar.fillAmount = 1f;
            }
        }

        private void UpdateProgressBar()
        {
            _levelProgressBar.DOFillAmount(_currentProgress, DelayUpdateProgressBar);
        }
    }
}
