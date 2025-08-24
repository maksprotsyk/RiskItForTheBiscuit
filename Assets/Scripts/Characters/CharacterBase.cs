using UnityEngine;

namespace Characters
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private MovementComponent _movementComponent;
        [SerializeField] private AttackComponent _attackComponent;

        private CharacterAnimationController _animationController;
        private Rigidbody2D _rigidbody;

        public MovementComponent Movement => _movementComponent;
        public AttackComponent Attack => _attackComponent;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the character.");
                return;
            }
            _animationController = new CharacterAnimationController(animator);

            _movementComponent.Init(_rigidbody, _animationController);
            _attackComponent.Init(_animationController);
        }

        private void FixedUpdate()
        {
            _movementComponent.UpdateMovementState(Time.fixedDeltaTime);
        }
        private void Update()
        {
            _attackComponent.UpdateAttackState(Time.deltaTime);
        }
    }
}
