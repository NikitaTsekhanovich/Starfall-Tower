using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Handlers
{
    public class ConvertatorTransformToPosition : MonoBehaviour
    {
        [SerializeField] private Transform[] _transformsBlocks;
        [SerializeField] private ConfigConvertPositionBlocks _configConvertPositionBlocks;
        [SerializeField] private PositionBlock[] _positionBlocks;
        
        [ContextMenu("Convert")]
        private void Convert()
        {
            _configConvertPositionBlocks.BasePositionBlocks.Clear();
            
            foreach (var transformBlock in _transformsBlocks)
            {
                var meshSize = transformBlock.GetComponent<MeshFilter>().sharedMesh.bounds.size;
                
                var newPositionBlock = new PositionBlock(
                    transformBlock.position,
                    transformBlock.rotation.eulerAngles,
                    transformBlock.localScale,
                    meshSize);
                
                _configConvertPositionBlocks.BasePositionBlocks.Add(newPositionBlock);
            }
            
            // foreach (var transformBlock in _positionBlocks)
            // {
            //     var newPositionBlock = new PositionBlock(
            //         transformBlock.BlockPosition,
            //         transformBlock.BlockRotation,
            //         transformBlock.BlockScale,
            //         Vector3.one,
            //         transformBlock.Row);
            //     
            //     _configConvertPositionBlocks.BasePositionBlocks.Add(newPositionBlock);
            // }
        }
    }
}
