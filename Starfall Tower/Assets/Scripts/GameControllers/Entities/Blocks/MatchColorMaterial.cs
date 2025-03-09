using System;
using UnityEngine;

namespace GameControllers.Entities.Blocks
{
    [Serializable]
    public class MatchColorMaterial
    {
        [field: SerializeField] public TypeColor TypeColor { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
    }
}
