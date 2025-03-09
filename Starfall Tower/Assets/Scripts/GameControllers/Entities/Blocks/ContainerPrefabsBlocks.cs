using System.Collections.Generic;
using UnityEngine;

namespace GameControllers.Entities.Blocks
{
    public class ContainerPrefabsBlocks : MonoBehaviour
    {
        [SerializeField] private Block[] _prefabsBlocks;
        
        public Dictionary<TypeBlock, Block> Blocks { get; private set; }

        private void Awake()
        {
            CreateDictionaryBlocks();
        }

        private void CreateDictionaryBlocks()
        {
            Blocks = new Dictionary<TypeBlock, Block>();

            foreach (var prefabBlock in _prefabsBlocks)
                Blocks[prefabBlock.TypeBlock] = prefabBlock;
        }
    }
}
