using UnityEngine;

namespace GameControllers.Entities.Balls
{
    public class Movement
    {
        private readonly Transform _transform;
        private readonly Rigidbody _rigidbody;
        
        public Movement(Transform transform, Rigidbody rigidbody)
        {
            _transform = transform;
            _rigidbody = rigidbody;
        }
        
        public void LaunchBall(Vector3 targetPosition)
        {
            _transform.SetParent(null);
            
            _rigidbody.useGravity = true;
            
            var startPosition = _transform.position;
            
            // Разница по осям (X, Y, Z)
            var direction = targetPosition - startPosition;
            
            // Время полета (например, 1 секунда, или можно вычислить на основе расстояния)
            // var timeToTarget = Mathf.Sqrt(2 * Mathf.Abs(direction.y) / Mathf.Abs(Physics.gravity.y));
            var timeToTarget = 1f;
            
            // Горизонтальная скорость
            var horizontalVelocity = new Vector3(direction.x / timeToTarget, 0, direction.z / timeToTarget);
            
            // Вертикальная скорость
            var verticalVelocity = (direction.y / timeToTarget) + 0.5f * Mathf.Abs(Physics.gravity.y) * timeToTarget;
            
            // Финальная скорость
            var finalVelocity = horizontalVelocity;
            finalVelocity.y = verticalVelocity;
            
            // // Множим на силу для получения нужной скорости
            // finalVelocity *= 2f;
            
            // Устанавливаем скорость мяча
            _rigidbody.velocity = finalVelocity;
        }
    }
}
