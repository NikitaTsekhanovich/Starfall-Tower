using TMPro;
using UnityEngine;
using Zenject;

namespace StoreControllers
{
    public class StoreDisplay : MonoBehaviour
    {
        [SerializeField] private StoreItem[] _storeItems;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private AudioSource _purchaseSound;
        
        [Inject] private ContainerAbilitiesConfigs _containerAbilitiesConfigs;

        private int _currentCoins;
        
        private void Start()
        {
            LoadStoreItems();
        }

        private void LoadStoreItems()
        {
            for (var i = 0; i < _containerAbilitiesConfigs.AbilitiesConfigs.Length; i++)
            {
                var config = _containerAbilitiesConfigs.AbilitiesConfigs[i];
                
                _storeItems[i].Init(
                    config.StoreIcon,
                    config.Cost,
                    config.CountAbility,
                    config.CountAbilityKey,
                    config.NameItem,
                    config.DescriptionItem,
                    CanBuyItem);
            }
        }

        private void UpdateCoinsText()
        {
            _coinsText.text = _currentCoins.ToString();
        }

        private bool CanBuyItem(int cost)
        {
            if (_currentCoins - cost >= 0)
            {
                _currentCoins -= cost;
                PlayerPrefs.SetInt(StoreSaveKeys.CoinsKey, _currentCoins);
                
                _purchaseSound.Play();
                UpdateCoinsText();
                return true;
            }
            
            return false;
        }

        public void ClickOpenStore()
        {
            _currentCoins = PlayerPrefs.GetInt(StoreSaveKeys.CoinsKey);
            UpdateCoinsText();
        }
    }
}
