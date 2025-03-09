using System;
using GameControllers.Abilities.Types;
using GameControllers.Input;
using UnityEngine;
using Zenject;

namespace GameControllers.Abilities.Activators
{
    public class MeteorShowerActivator : AbilityActivator
    {
        [SerializeField] private MeteorShower _meteorShower;
        
        [Inject] private InputController _inputController;

        private void Start()
        {
            _inputController.OnChooseTargetMeteor += StartAbility;
            Init();
        }

        protected override void UseAbility()
        {
            _inputController.SetAiming(true);
        }

        private void StartAbility()
        {
            _meteorShower.StartAbility(_inputController.TargetClick);
            _inputController.SetAiming(false);
        }

        private void OnDestroy()
        {
            _inputController.OnChooseTargetMeteor -= StartAbility;
        }
    }
}
