using UnityEngine;

namespace GlobalSystems
{
    public class SaverGlobalSystems : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
