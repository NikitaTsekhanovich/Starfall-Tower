using StoreControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameControllers.Abilities
{
    public abstract class AbilityActivator : MonoBehaviour
    {
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _countAbilityText;
        [SerializeField] private ConfigAbility _configAbility;

        private int _currentCount;

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            _currentCount = _configAbility.CountAbility;
            _countAbilityText.text = _currentCount.ToString();
 
            if (_currentCount <= 0) _actionButton.interactable = false;
        }

        public void ClickUseAbility()
        {
            if (_currentCount - 1 < 0)
            {
                return;
            }
            
            UseAbility();
            
            _currentCount--;
            _countAbilityText.text = _currentCount.ToString();
            PlayerPrefs.SetInt(_configAbility.CountAbilityKey, _currentCount);
        }

        public void ChangeStateActionButton(bool isInteractable)
        {
            if (_currentCount <= 0) 
                _actionButton.interactable = false;
            else
                _actionButton.interactable = isInteractable;
        }
        
        protected abstract void UseAbility();
    }
}
