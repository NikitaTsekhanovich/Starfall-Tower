using System;
using System.Collections.Generic;

namespace Levels
{
    public class ContainerLevelsConfigs
    {
        private Queue<LevelConfig> _levelsQueue;
        
        public LevelConfig CurrentLevel { get; private set; }
        public readonly LevelConfig[] LevelsConfigs;

        public ContainerLevelsConfigs(LevelConfig[] levelsConfigs)
        {
            LevelsConfigs = levelsConfigs;
        }

        public void SetCurrentLevel(int index)
        {
            CurrentLevel = LevelsConfigs[index];
        }

        public void CreateLevelsQueue()
        {
            var random = new Random();
            var length = LevelsConfigs.Length;
            var tempLevels = new LevelConfig[length];
            Array.Copy(LevelsConfigs, tempLevels, length);
            
            while (length > 1)
            {
                length--;
                var newIndex = random.Next(length + 1);
                (tempLevels[newIndex], tempLevels[length]) = (tempLevels[length], tempLevels[newIndex]);
            }
            
            _levelsQueue = new Queue<LevelConfig>(tempLevels);
            CurrentLevel = _levelsQueue.Dequeue();
        }

        public void ChooseNextRandomLevel()
        {
            if (_levelsQueue.Count > 0)
            {
                CurrentLevel = _levelsQueue.Dequeue();
            }
            else
            {
                CreateLevelsQueue();
            }
        }
    }
}
