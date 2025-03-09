using System;
using TMPro;
using UnityEngine;

namespace GameControllers.Logic.Components
{
    public class BallNumberController
    {
        private readonly TMP_Text _ballsCountText;
        private readonly Action _onLose;
        
        public int CurrentCountBalls { get; private set; }

        public BallNumberController(TMP_Text ballsCountText, Action onLose, int countBalls)
        {
            _ballsCountText = ballsCountText;
            _onLose = onLose;
            CurrentCountBalls = countBalls;
            
            _ballsCountText.text = $"{CurrentCountBalls}";
        }
        
        public void DecreaseBalls()
        {
            CurrentCountBalls--;
            _ballsCountText.text = $"{CurrentCountBalls}";
            
            if (CurrentCountBalls <= 0)
                _onLose.Invoke();
        }
    }
}
