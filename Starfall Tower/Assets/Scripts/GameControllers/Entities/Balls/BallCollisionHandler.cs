using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Entities.Balls
{
    public class BallCollisionHandler : MonoBehaviour
    {
        [SerializeField] private Ball _ball;
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<ICanTakeDamage>(out var takeableDamageEntity))
            {
                if (takeableDamageEntity.CanTakeDamage(_ball.TypeColor))
                    _ball.DoDestroy();
            }
        }
    }
}
