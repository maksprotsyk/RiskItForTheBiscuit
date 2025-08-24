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

        public MovementComponent Movement => _movementComponent;
        public AttackComponent Attack => _attackComponent;
        public HealthComponent Health => _healthComponent;
        public CharacterAnimationController AnimationController => _animationController;

        private void Awake()
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the character.");
                return;
            }
            _animationController = new CharacterAnimationController(animator);

            var allComponents = new List<ICharacterComponent> { _movementComponent, _attackComponent, _healthComponent };
            foreach (ICharacterComponent item in allComponents)
            {
                item.Init(this);
            }
        }

        private void FixedUpdate()
        {
            _movementComponent.UpdateComponent(Time.fixedDeltaTime);
        }
        private void Update()
        {
            _attackComponent.UpdateComponent(Time.deltaTime);
            _healthComponent.UpdateComponent(Time.deltaTime);
        }
    }
}
