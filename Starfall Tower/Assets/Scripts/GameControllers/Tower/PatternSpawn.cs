using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Tower
{
    [CreateAssetMenu(fileName = "PatternSpawn", menuName = "Patterns block spawn/ Pattern")]
    public class PatternSpawn : ScriptableObject
    {
        [field: SerializeField] public TypeBlock TypeBlock { get; private set; }
        [field: SerializeField] public int BlocksHeight { get; private set; }
        [field: SerializeField] public int BaseBlocksHeight { get; private set; }
        [field: SerializeField] public PositionBlock[] BasePositionBlocks { get; private set; }
    }
}
