using GameControllers.Factories.Properties;
using UnityEngine;
using Zenject;

namespace GameControllers.Abilities.Activators
{
    public class UniversalBallActivator : AbilityActivator
    {
        [SerializeField] private Material _universalMaterial;
        
        [Inject] private ICanGetBall _ballFactory;
        
        protected override void UseAbility()
        {
            _ballFactory.GetCurrentBall().BecomeUniversal(_universalMaterial);
        }
    }
}
