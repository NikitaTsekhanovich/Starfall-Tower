using System.Collections.Generic;
using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Handlers
{
    [CreateAssetMenu(fileName = "ConfigConvertPositionBlocks", menuName = "Helpers Configs/ Config")]
    public class ConfigConvertPositionBlocks : ScriptableObject
    {
        public List<PositionBlock> BasePositionBlocks;
    }
}
