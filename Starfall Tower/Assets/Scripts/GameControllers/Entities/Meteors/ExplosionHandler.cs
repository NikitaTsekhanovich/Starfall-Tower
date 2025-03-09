using System.Collections.Generic;
using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Entities.Meteors
{
    public class ExplosionHandler : MonoBehaviour
    {
        private readonly HashSet<ICanTakeDamage> _takeableDamageObjects = new ();
        
        public void Explosion()
        {
            foreach (var takeableDamageObject in _takeableDamageObjects)
            {
                if (takeableDamageObject != null)
                    takeableDamageObject.CanTakeDamage(TypeColor.Universal);
            }
            
            _takeableDamageObjects.Clear();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICanTakeDamage takeableDamageObject))
                _takeableDamageObjects.Add(takeableDamageObject);
        }
    }
}
