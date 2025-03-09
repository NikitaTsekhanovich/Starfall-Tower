using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControllers.Entities.Blocks
{
    public class Block : MonoBehaviour, ICanTakeDamage
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AudioSource _destroySound;
        [SerializeField] private ParticleSystem _destroyParticles;
        private readonly HashSet<Block> _neighboringBlocks = new ();
        private TypeColor _unfixedColor;
        private Material _unfixedMaterial;
        private Collider _collider;
        private Renderer _particleRenderer;

        [field: SerializeField] public TypeBlock TypeBlock { get; private set; }
        public TypeColor TypeColor { get; private set; }
        public bool IsDestroyed { get; private set; }
        public event Action<Block> OnBlockDestroyed; 

        public void Init(MatchColorMaterial unfixedColorMaterial, MatchColorMaterial fixedColorMaterial = null)
        {
            _collider = GetComponent<Collider>();
            _particleRenderer = _destroyParticles.GetComponent<Renderer>();
            _particleRenderer.material = unfixedColorMaterial.Material;
            
            if (fixedColorMaterial != null)
            {
                FixedBlock();

                _unfixedColor = unfixedColorMaterial.TypeColor;
                _unfixedMaterial = unfixedColorMaterial.Material;
                
                TypeColor = fixedColorMaterial.TypeColor;
                _meshRenderer.material = fixedColorMaterial.Material;
            }
            else
            {
                TypeColor = unfixedColorMaterial.TypeColor;
                _meshRenderer.material = unfixedColorMaterial.Material;
            }
        }

        public void UnfixedBlock()
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            // _rigidbody.isKinematic = false;
            
            TypeColor = _unfixedColor;
            _meshRenderer.material = _unfixedMaterial;
        }

        private void FixedBlock()
        {
            _rigidbody.constraints = 
                RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezePositionY | 
                RigidbodyConstraints.FreezePositionZ;
            
            _rigidbody.constraints |= 
                RigidbodyConstraints.FreezeRotationX | 
                RigidbodyConstraints.FreezeRotationY | 
                RigidbodyConstraints.FreezeRotationZ;
            // _rigidbody.isKinematic = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out Block block) && block.TypeColor == TypeColor)
            {
                _neighboringBlocks.Add(block);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out Block block))
            {
                _neighboringBlocks.Remove(block);
            }
        }

        public bool CanTakeDamage(TypeColor typeColor)
        {
            if (typeColor == TypeColor || (typeColor == TypeColor.Universal && TypeColor != TypeColor.Black))
            {
                DoDestroy();
                return true;
            }

            return false;
        }

        public void DoDestroy()
        {
            if (IsDestroyed) return;
            
            IsDestroyed = true;
            
            _destroySound.Play();
            OnBlockDestroyed?.Invoke(this);
            _meshRenderer.enabled = false;
            _collider.enabled = false;
            _destroyParticles.Play();

            foreach (var neighboringBlock in _neighboringBlocks)
                neighboringBlock?.CanTakeDamage(TypeColor);

            StartCoroutine(DelayDestroy());
        }

        private IEnumerator DelayDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
