using GameControllers.Tower;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Levels Configs/ Config")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public int Index { get; private set; }
        [field: SerializeField] public int BallCount { get; private set; }
        [field: SerializeField] public Sprite PreviewLevelSprite { get; private set; }
        [field: SerializeField] public Sprite LevelNumberSprite { get; private set; }
        [field: SerializeField] public LevelItem LevelItem { get; private set; }
        [field: SerializeField] public PatternSpawn PatternSpawn { get; private set; }

        public string StateLevelKey => $"{LevelSaveKeys.StateLevelKey}{Index}";
        
        public TypeStateLevel StateLevel =>
            (TypeStateLevel)PlayerPrefs.GetInt(StateLevelKey);
    }
}
