using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlobalSystems
{
    public class SceneDataLoader
    {
        private readonly LoadingScreenController _loadingScreenController;
        private string _nameScene;
        
        public ModeGame ModeGame { get; private set; }
        
        public SceneDataLoader(LoadingScreenController loadingScreenController)
        {
            _loadingScreenController = loadingScreenController;
            CheckFirstLaunch();
            ChangeScene("MainMenu");
        }
        
        public void ChangeScene(string nameScene, ModeGame modeGame = ModeGame.Levels)
        {
            ModeGame = modeGame;
            _nameScene = nameScene;
            _loadingScreenController.StartAnimationFade(LoadScene);
        }

        private void LoadScene()
        {
            SceneManager.LoadSceneAsync(_nameScene);
        }
        
        private void CheckFirstLaunch()
        {
            if (PlayerPrefs.GetInt(LaunchSaveKeys.LaunchTypeKey) == (int)TypeLaunch.IsFirst)
            {
                PlayerPrefs.SetInt($"{LevelSaveKeys.StateLevelKey}{0}", (int)TypeStateLevel.IsOpen);
                PlayerPrefs.SetInt(LaunchSaveKeys.LaunchTypeKey, (int)TypeLaunch.IsNotFirst);
            }
        }
    }
}
