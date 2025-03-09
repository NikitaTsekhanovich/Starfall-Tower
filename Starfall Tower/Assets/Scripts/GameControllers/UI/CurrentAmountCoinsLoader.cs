using StoreControllers;
using TMPro;
using UnityEngine;

namespace GameControllers.UI
{
    public class CurrentAmountCoinsLoader : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currentCoinsText;

        private void OnEnable()
        {
            _currentCoinsText.text = PlayerPrefs.GetInt(StoreSaveKeys.CoinsKey).ToString();
        }
    }
}
