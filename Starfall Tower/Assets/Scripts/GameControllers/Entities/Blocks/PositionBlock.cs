using System;
using UnityEngine;

namespace GameControllers.Entities.Blocks
{
    [Serializable]
    public class PositionBlock
    {
        [field: SerializeField] public int Row { get; private set; }
        [field: SerializeField] public Vector3 BlockPosition { get; private set; }
        [field: SerializeField] public Vector3 BlockRotation { get; private set; }
        [field: SerializeField] public Vector3 BlockScale { get; private set; }
        [field: SerializeField] public Vector3 MeshSize { get; private set; }

        public PositionBlock(
            Vector3 position,
            Vector3 rotation,
            Vector3 scale,
            Vector3 meshSize,
            int row = 0)
        {
            BlockPosition = position;
            BlockRotation = rotation;
            BlockScale = scale;
            MeshSize = meshSize;
            Row = row;
        }
    }
}
