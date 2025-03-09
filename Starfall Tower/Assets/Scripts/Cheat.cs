using StoreControllers;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    private int _counter;

    public void ClickLogo()
    {
        _counter++;
        if (_counter % 10 == 0)
        {
            Debug.Log("Cheater!");
            var currentCoins = PlayerPrefs.GetInt($"{StoreSaveKeys.CoinsKey}");
            PlayerPrefs.SetInt($"{StoreSaveKeys.CoinsKey}", currentCoins + 1000);
        }
    }
}