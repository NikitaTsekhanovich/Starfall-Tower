using GameControllers.Tower;
using StoreControllers;
using UnityEngine;

namespace GameControllers.Systems
{
    public class CoinsHandler : MonoBehaviour
    {
        private int _currentCoins;
        
        private void OnEnable()
        {
            RowCollisionHandler.OnRowCleared += IncreaseCoins;

            _currentCoins = PlayerPrefs.GetInt(StoreSaveKeys.CoinsKey);
        }

        private void OnDisable()
        {
            RowCollisionHandler.OnRowCleared -= IncreaseCoins;
        }

        private void IncreaseCoins()
        {
            _currentCoins++;
            PlayerPrefs.SetInt(StoreSaveKeys.CoinsKey, _currentCoins);
        }
    }
}
