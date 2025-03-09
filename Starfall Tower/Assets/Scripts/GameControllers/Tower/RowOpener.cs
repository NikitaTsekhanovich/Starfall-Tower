using System;
using System.Collections.Generic;
using GameControllers.Entities.Blocks;

namespace GameControllers.Tower
{
    public class RowOpener : IDisposable
    {
        private readonly Dictionary<int, List<Block>> _blocksRow = new ();
        private int _lastRowIndex;
        
        public RowOpener()
        {
            SubscribeActions();
        }

        public void AddBlock(int row, Block block)
        {
            if (_blocksRow.ContainsKey(row))
                _blocksRow[row].Add(block);
            else
                _blocksRow[row] = new List<Block> { block };

            _lastRowIndex = row;
        }

        public void Dispose()
        {
            RowCollisionHandler.OnRowCleared -= OpenRow;
            _blocksRow.Clear();
        }

        private void OpenRow()
        {
            if (!_blocksRow.TryGetValue(_lastRowIndex, out var blockRow)) return;
            
            foreach (var block in blockRow)
                block.UnfixedBlock();
            
            _blocksRow[_lastRowIndex].Clear();
            _lastRowIndex--;
        }
        
        private void SubscribeActions()
        {
            RowCollisionHandler.OnRowCleared += OpenRow;
        }
    }
}
