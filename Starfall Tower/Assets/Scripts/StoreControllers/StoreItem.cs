using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StoreControllers
{
    public class StoreItem : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        private int _cost;
        private int _currentCountAbility;
        private string _countAbilityKey;
        private Func<int, bool> _canBuyItem;
        
        public void Init(
            Sprite icon,
            int cost, 
            int count, 
            string countAbilityKey,
            string nameItem,
            string descriptionItem,
            Func<int, bool> canBuyItem)
        {
            _iconImage.sprite = icon;
            _costText.text = $"{cost}";
            _countText.text = $"{count}";
            _cost = cost;
            _currentCountAbility = count;
            _countAbilityKey = countAbilityKey;
            _canBuyItem = canBuyItem;
            _nameText.text = nameItem;
            _descriptionText.text = descriptionItem;
        }

        public void ClickBuy()
        {
            if (_canBuyItem.Invoke(_cost))
            {
                _currentCountAbility++;
                PlayerPrefs.SetInt(_countAbilityKey, _currentCountAbility);
                _countText.text = _currentCountAbility.ToString();
            }
        }
    }
}
