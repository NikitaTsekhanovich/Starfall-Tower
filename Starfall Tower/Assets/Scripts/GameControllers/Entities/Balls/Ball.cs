using DG.Tweening;
using GameControllers.Input;
using UnityEngine;
using Zenject;

namespace GameControllers.Entities.Balls
{
    public abstract class Ball : MonoBehaviour
    {
        [SerializeField] private AudioSource _throwSound;
        
        private const float LifeTime = 5f;
        private float _currentLifeTime;
        private Collider _collider;
        private bool _lifeCounterWorks;
        
        [Inject] private InputController _inputController;

        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Movement Movement { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public TypeColor TypeColor { get; protected set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Transform = GetComponent<Transform>();
            MeshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            
            SpawnInit();
            CreateComponents();
        }

        private void Update()
        {
            StartLifeCounter();
        }

        public void AlreadyThrow()
        {
            _collider.enabled = true;
            SubscribeEvents();
        }
        
        public void BecomeUniversal(Material universalMaterial)
        {
            TypeColor = TypeColor.Universal;
            MeshRenderer.material = universalMaterial;
        }

        public void DoDestroy()
        {
            UnsubscribeEvents();
            Destroy(gameObject);
        }
        
        protected abstract void SpawnInit();

        private void StartLifeCounter()
        {
            if (!_lifeCounterWorks) return;
            
            _currentLifeTime += Time.deltaTime;
            
            if (_currentLifeTime >= LifeTime)
            {
                _lifeCounterWorks = false;
                
                DOTween.Sequence()
                    .Append(transform.DOScale(Vector3.zero, 1f))
                    .AppendCallback(DoDestroy);
            }
        }
        
        private void SubscribeEvents()
        {
            _inputController.OnChooseTargetThrow += ChooseTargetThrow;
        }
        
        private void CreateComponents()
        {
            Movement = new Movement(Transform, Rigidbody);
        }
        
        private void UnsubscribeEvents()
        {
            _inputController.OnChooseTargetThrow -= ChooseTargetThrow;
        }

        private void ChooseTargetThrow()
        {
            _throwSound.Play();
            _inputController.OnChooseTargetThrow -= ChooseTargetThrow;
            Movement.LaunchBall(_inputController.TargetClick);
            _lifeCounterWorks = true;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
    }
}
