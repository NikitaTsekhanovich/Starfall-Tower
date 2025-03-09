using UnityEngine;

namespace MusicSystem
{
    public class BackgroundMusicSaver : MonoBehaviour
    {
        private void Awake()
        {
            var objects = Resources.FindObjectsOfTypeAll(typeof(BackgroundMusicSaver));

            if (objects.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
