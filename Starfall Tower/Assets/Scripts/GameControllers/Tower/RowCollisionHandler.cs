using System;
using System.Collections.Generic;
using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Tower
{
    public class RowCollisionHandler : MonoBehaviour
    {
        private readonly HashSet<Block> _blocks = new ();
        private BoxCollider _boxCollider;
        private Action _winAction;

        public static Action OnRowCleared;

        public void Init(float newSizeY, Action winAction)
        {
            _boxCollider = GetComponent<BoxCollider>();
            _winAction = winAction;
            
            _boxCollider.size = new Vector3(
                _boxCollider.size.x, 
                newSizeY, 
                _boxCollider.size.z);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Block>(out var block))
            {
                block.OnBlockDestroyed += RemoveBlock;
                _blocks.Add(block);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Block>(out var block))
            {
                RemoveBlock(block);
            }
        }

        private void RemoveBlock(Block block)
        {
            block.OnBlockDestroyed -= RemoveBlock;
            _blocks.Remove(block);
            
            if (_blocks.Count <= 0)
            {
                OnRowCleared?.Invoke();
                _winAction?.Invoke();
                
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            foreach (var block in _blocks)
                block.OnBlockDestroyed -= RemoveBlock;
            
            _blocks.Clear();
        }
    }
}
