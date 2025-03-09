using System.Collections;
using GameControllers.Entities;
using GameControllers.Entities.Balls.Types;
using GameControllers.Factories.Properties;
using GameControllers.Input;
using GameControllers.Logic;
using UnityEngine;
using Zenject;

namespace GameControllers.Abilities
{
    public class AbilityBlockController : MonoBehaviour
    {
        [SerializeField] private GameObject _abilitiesBlock;
        [SerializeField] private GameObject _background;
        [SerializeField] private GameObject _fadeScreen;
        [SerializeField] private AbilityActivator _planetActivator;
        [SerializeField] private AbilityActivator _meteorsActivator;
        [SerializeField] private AbilityActivator _universalActivator;
        
        [Inject] private GameController _gameController;
        [Inject] private InputController _inputController;
        [Inject] private ICanGetBall _ballFactory;
        
        private bool _isOpen;
        
        public void ClickChangeStateBlock()
        {
            _isOpen = !_isOpen;
            _abilitiesBlock.SetActive(_isOpen);
            _background.SetActive(_isOpen);
            _fadeScreen.SetActive(_isOpen);
            
            StartCoroutine(WaitChangeState());
        }

        private IEnumerator WaitChangeState()
        {
            yield return new WaitForEndOfFrame();

            var currentBall = _ballFactory.GetCurrentBall();
            
            var isUniversalBall = currentBall.TryGetComponent(out StandardBall standardBall) && 
                                  standardBall.TypeColor == TypeColor.Universal;
            var isPlanet = currentBall.TryGetComponent(out Planet planet);
            
            _universalActivator.ChangeStateActionButton(!(isUniversalBall || isPlanet));
            _planetActivator.ChangeStateActionButton(!(isUniversalBall || isPlanet));
            _meteorsActivator.ChangeStateActionButton(!_inputController.IsThrowMeteors);
            
            _gameController.ChooseAbility(_isOpen);
        }
    }
}
