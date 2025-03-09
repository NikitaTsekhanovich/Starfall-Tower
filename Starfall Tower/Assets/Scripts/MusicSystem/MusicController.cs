using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MusicSystem
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private Sprite _musicOnImage;
        [SerializeField] private Sprite _musicOffImage;
        [SerializeField] private Sprite _effectsOnImage;
        [SerializeField] private Sprite _effectsOffImage;
        [SerializeField] private Image _musicIcon; 
        [SerializeField] private Image _soundIcon;
        private ModeMusic _musicIsOn;
        private ModeMusic _effectsIsOn;
        private const int volumeOn = 0;
        private const int volumeOff = -80;
        private const string musicMixerName = "Music";
        private const string effectsMixerName = "SoundEffects";

        public static MusicController Instance;

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
            
            LoadData();
            LoadIcons();
        }

        private void LoadData()
        {
            _musicIsOn = (ModeMusic)PlayerPrefs.GetInt(MusicDataKeys.MusicIsOnKey);
            _effectsIsOn = (ModeMusic)PlayerPrefs.GetInt(MusicDataKeys.EffectsIsOnKey);
        }

        public void LoadIcons()
        {
            ChangeVolume(_musicIsOn, musicMixerName, _musicIcon,
                _musicOnImage, _musicOffImage);
            ChangeVolume(_effectsIsOn, effectsMixerName, _soundIcon,
                _effectsOnImage, _effectsOffImage);
        }

        public void CheckMusicState(Image currentMusicImage)
        {
            ChangeState(ref _musicIsOn, MusicDataKeys.MusicIsOnKey);
            ChangeVolume(_musicIsOn, musicMixerName, currentMusicImage,
                _musicOnImage, _musicOffImage);
        }

        public void CheckSoundEffectsState(Image currentEffectsImage)
        {
            ChangeState(ref _effectsIsOn, MusicDataKeys.EffectsIsOnKey);
            ChangeVolume(_effectsIsOn, effectsMixerName, currentEffectsImage,
                _effectsOnImage, _effectsOffImage);
        }

        private void ChangeState(ref ModeMusic mode, string key)
        {
            mode = (ModeMusic)(((int)mode + (int)ModeMusic.Off) % 2);
            PlayerPrefs.SetInt(key, (int)mode);
        }

        private void ChangeVolume(ModeMusic isOn, string mixerName, Image currentImage,
            Sprite onImage, Sprite offImage)
        {
            if (isOn == ModeMusic.On)
            {
                _mixer.SetFloat(mixerName, volumeOn);
                currentImage.sprite = onImage;
            }
            else
            {
                _mixer.SetFloat(mixerName, volumeOff);
                currentImage.sprite = offImage;
            }
        }
    }
}
