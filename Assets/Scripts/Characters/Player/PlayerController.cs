using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterBase))]
    public class PlayerController : MonoBehaviour
    {
        private InputReader _inputReader;
        private CharacterBase _character;

        bool _isRunningRequested = false;
        Vector2 _movementDirection = Vector2.zero;

        ///////////////////////////////////////////////////////

        private void Awake()
        {
            _inputReader = new InputReader();
            _character = GetComponent<CharacterBase>();
            _isRunningRequested = false;

			AddListeners();
        }

        ///////////////////////////////////////////////////////

        private void OnDestroy()
        {
            RemoveListeners();
        }

        ///////////////////////////////////////////////////////

        private void AddListeners()
        {
            RemoveListeners();

            _inputReader.MoveEvent += OnMoveEvent;
            _inputReader.AttackEvent += _character.Weapon.PerformAttack;
            _inputReader.RunningState += OnRunningStateChanged;
        }

        ///////////////////////////////////////////////////////
        
        private void RemoveListeners()
        {
            _inputReader.MoveEvent -= OnMoveEvent;
            _inputReader.AttackEvent -= _character.Weapon.PerformAttack;
            _inputReader.RunningState -= OnRunningStateChanged;
        }

        ///////////////////////////////////////////////////////

        private void OnMoveEvent(Vector2 direction)
        {
            _movementDirection = direction;
            UpdateMovement();
        }

        ///////////////////////////////////////////////////////

        private void OnRunningStateChanged(bool isRunningRequested)
        {
            _isRunningRequested = isRunningRequested;
            UpdateMovement();
        }

        ///////////////////////////////////////////////////////

        private void UpdateMovement()
        {
            if (_movementDirection.sqrMagnitude > float.Epsilon)
            {
                _character.Movement.SetLookDirection(_movementDirection);
                _character.Movement.SetPrefferedMovingState(_isRunningRequested ? MovementComponent.MovingState.Running : MovementComponent.MovingState.Walking);
            }
            else
            {
                _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
            }
        }

    }
}
