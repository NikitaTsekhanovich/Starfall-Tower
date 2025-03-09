using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Entities
{
    public class ContainerColors : MonoBehaviour
    {
        [field: SerializeField] public MatchColorMaterial[] MaterialsBlocks { get; private set; }
        [field: SerializeField] public MatchColorMaterial UnfixedMaterialBlock { get; private set; }
    }
}
