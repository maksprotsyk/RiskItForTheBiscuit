using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private MovementComponent _movementComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private HealthComponent _healthComponent;

        private CharacterAnimationController _animationController;
        private List<ICharacterComponent> _characterComponents;

        public MovementComponent Movement => _movementComponent;
        public AttackComponent Attack => _attackComponent;
        public HealthComponent Health => _healthComponent;
        public CharacterAnimationController AnimationController => _animationController;

        public void OnAttackAnimationStarted()
        {
            _attackComponent.SetProjectileState(true);
        }

        public void OnAttackAnimationFinished()
        {
            _attackComponent.SetProjectileState(false);
        }

        private void Awake()
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the character.");
                return;
            }
            _animationController = new CharacterAnimationController(animator);

            _characterComponents = new List<ICharacterComponent> { _movementComponent, _attackComponent, _healthComponent };
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.Init(this);
            }
        }

        private void FixedUpdate()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.FixedUpdateComponent(Time.fixedDeltaTime);
            }
        }
        private void Update()
        {
            foreach (ICharacterComponent comp in _characterComponents)
            {
                comp.UpdateComponent(Time.deltaTime);
            }
        }
    }
}
