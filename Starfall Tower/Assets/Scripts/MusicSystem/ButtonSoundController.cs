using UnityEngine;

namespace MusicSystem
{
    public class ButtonSoundController : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundButton;

        public void StartSound()
        {
            _soundButton.Play();
        }
    }
}
