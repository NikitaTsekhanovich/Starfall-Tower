using System;
using System.Collections;
using DG.Tweening;
using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Entities.Meteors
{
    public class Meteor : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private ExplosionHandler _explosionHandler;
        [SerializeField] private ParticleSystem _explodeParticles;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _explodeCollider;
        [SerializeField] private Vector3 _scale;
        [SerializeField] private Transform _movePoint;
        [SerializeField] private AudioSource _destroySound;
        
        private Action _cameraShakeAction;
        private bool _alreadyDestroy;
        private float _currentTime;
        private const float LifeTime = 6f;
        
        public void Init(Vector3 targetPosition, Action cameraShakeAction)
        {
            _cameraShakeAction = cameraShakeAction;

            DOTween.Sequence()
                .Append(transform.DOScale(_scale, 1.5f))
                .AppendCallback(() => _movePoint.position = targetPosition);
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            
            if (_movePoint != null)
                transform.position = Vector3.Lerp(
                    transform.position, 
                    _movePoint.position, 
                    _speed * Time.deltaTime);

            if (_currentTime >= LifeTime && !_alreadyDestroy)
                DoDestroy();
        }

        private void OnCollisionEnter(Collision collision)
        {
            ExplosionDestroy();
        }

        private void ExplosionDestroy()
        {
            if (_alreadyDestroy) return;
            
            _alreadyDestroy = true;
            
            _collider.enabled = false;
            _explodeCollider.enabled = false;
            _cameraShakeAction.Invoke();
            _explosionHandler.Explosion();
            
            DoDestroy();
        }

        private void DoDestroy()
        {
            _alreadyDestroy = true;
            _meshRenderer.enabled = false;
            _destroySound.Play();
            _explodeParticles.Play();
            StartCoroutine(DelayedDestroy());
        }

        private IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
