using UnityEngine;
using Zenject;

namespace GameControllers.Entities.Balls.Types
{
    public class StandardBall : Ball
    {
        [Inject] private ContainerColors _containerColors;
        
        protected override void SpawnInit()
        {
            ChooseTypeColor();
        }
         
        private void ChooseTypeColor()
        {
            var randomColor = _containerColors.MaterialsBlocks[
                Random.Range(0, _containerColors.MaterialsBlocks.Length)];
            
            MeshRenderer.material = randomColor.Material;
            TypeColor = randomColor.TypeColor;
        }
    }
}
