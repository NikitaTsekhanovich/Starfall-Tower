using GameControllers.Camera;
using GameControllers.Entities;
using GameControllers.Entities.Blocks;
using GameControllers.Logic;
using GameControllers.Systems;
using Levels;
using UnityEngine;
using Zenject;

namespace GameControllers.Tower
{
    public class TowerSpawner : MonoBehaviour
    {
        [SerializeField] private RowCollisionHandler _rowCollisionHandler;
        [SerializeField] private RowCollisionHandler _firstRowCollisionHandler;
        
        private const int UnfixedCountBlocks = 5;
        private const float ScaleMultiplierCollisionHandler = 0.8f;
        private RowOpener _rowOpener;
        
        [Inject] private ContainerPrefabsBlocks _containerPrefabs;
        [Inject] private ContainerColors _containerColors;
        [Inject] private CameraHandler _cameraHandler;
        [Inject] private SystemsController _systemsController;
        [Inject] private GameController _gameController;
        [Inject] private ContainerLevelsConfigs _containerLevelsConfigs;
        
        public const int IndexVictoryBorderRow = 3;

        private void Start()
        {
            _rowOpener = new RowOpener();
            SpawnTower(_containerLevelsConfigs.CurrentLevel.PatternSpawn);
            _systemsController.RegistrationDisposable(_rowOpener);
        }

        private void SpawnTower(PatternSpawn patternSpawn)
        {
            var scaleY = patternSpawn.BasePositionBlocks[0].BlockScale.y *
                         patternSpawn.BasePositionBlocks[0].MeshSize.y;
            _cameraHandler.SetStartHeight(scaleY, patternSpawn.BlocksHeight);

            CreateBlocks(patternSpawn, scaleY);
            CreateRowCollisionHandlers(patternSpawn, scaleY);
        }

        private void CreateBlocks(PatternSpawn patternSpawn, float scaleY)
        {
            for (var i = 0; i < patternSpawn.BlocksHeight; i += patternSpawn.BaseBlocksHeight)
            {
                foreach (var position in patternSpawn.BasePositionBlocks)
                {
                    var indexRow = i + position.Row;
                    
                    var newPosition = new Vector3(
                        position.BlockPosition.x * position.MeshSize.x,
                        scaleY / 2 + scaleY * indexRow,
                        position.BlockPosition.z * position.MeshSize.z);
                    
                    var newBlock = Instantiate(
                        _containerPrefabs.Blocks[patternSpawn.TypeBlock], 
                        newPosition, 
                        Quaternion.Euler(position.BlockRotation));
                    
                    newBlock.transform.localScale = position.BlockScale;

                    var randomIndexColorMaterial = Random.Range(0, _containerColors.MaterialsBlocks.Length);
                    var colorMaterial = _containerColors.MaterialsBlocks[randomIndexColorMaterial];
                    
                    if (patternSpawn.BlocksHeight - indexRow - 1 <= UnfixedCountBlocks)
                    {
                        newBlock.Init(colorMaterial);
                    }
                    else
                    {
                        newBlock.Init(colorMaterial, _containerColors.UnfixedMaterialBlock);
                        _rowOpener.AddBlock(indexRow, newBlock);
                    }
                }
            }
        }

        private void CreateRowCollisionHandlers(PatternSpawn patternSpawn, float scaleY)
        {
            for (var i = IndexVictoryBorderRow; i < patternSpawn.BlocksHeight; i++)
            {
                var newPosition = new Vector3(
                    0,
                    scaleY / 2 + scaleY * i,
                    0);

                if (i == IndexVictoryBorderRow)
                {
                    var newCollider = Instantiate(_firstRowCollisionHandler, newPosition, Quaternion.identity);
                
                    newCollider.Init(
                        scaleY * ScaleMultiplierCollisionHandler,
                        _gameController.DecreaseRow);
                }
                else
                {
                    var newCollider = Instantiate(_rowCollisionHandler, newPosition, Quaternion.identity);
                
                    newCollider.Init(
                        scaleY * ScaleMultiplierCollisionHandler,
                        _gameController.DecreaseRow);
                }
            }
        }
    }
}
