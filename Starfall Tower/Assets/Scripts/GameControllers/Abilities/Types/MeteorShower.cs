using System.Collections;
using GameControllers.Camera;
using GameControllers.Entities.Meteors;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace GameControllers.Abilities.Types
{
    public class MeteorShower : MonoBehaviour
    {
        [SerializeField] private Meteor[] _meteorsPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private AudioSource _spawnSound;
        
        [Inject] private CameraHandler _cameraHandler;
        
        private const float DelaySpawn = 1.5f;
        private const int CountMeteors = 4;
        
        public void StartAbility(Vector3 targetPosition)
        {
            StartCoroutine(SpawnMeteors(targetPosition));
        }

        private IEnumerator SpawnMeteors(Vector3 targetPosition)
        {
            for (var i = 0; i < CountMeteors; i++)
            {
                _spawnSound.Play();
                var randomPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
                var randomMeteor = _meteorsPrefab[Random.Range(0, _meteorsPrefab.Length)];

                var newMeteor = Instantiate(randomMeteor, randomPoint, Random.rotation);
                newMeteor.Init(targetPosition, _cameraHandler.Shake);
                
                yield return new WaitForSeconds(DelaySpawn);
            }
        }
    }
}
