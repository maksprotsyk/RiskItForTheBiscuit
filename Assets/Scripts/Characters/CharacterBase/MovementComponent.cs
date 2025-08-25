using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{

    [Serializable]
    public struct MovementParameters
    {
        public float MovementSpeed;
        public float RunningSpeed;
        public float StaminaTotalAmount;
        public float StaminaRegenRate;
        public float StaminaWalkRegenRate;
        public float StaminaDepletionRate;
        public float StaminaDepletionThreshold;
    }

    [Serializable]
    public class MovementComponent: ICharacterComponent
    {
        public enum MovingState
        {
            Idle = 0,
            Walking = 1,
            Running = 2
        }

        [SerializeField] private MovementParameters _movementParameters;

        private Rigidbody2D _rigidBody;
        private CharacterBase _character;

        private Dictionary<MovingState, float> _statesSpeed;
        private Dictionary<MovingState, float> _statesStaminaChangeRate;

        private MovingState _currentState;
        private Vector2 _movementDirection;
        private float _currentStamina;
        private bool _isRunningRequested;
        private bool _isDepleted;
        private bool _isMoving;

        public float Stamina
        {
            get
            {
                return _currentStamina;
            }
            private set
            {
                _currentStamina = Mathf.Clamp(value, 0, _movementParameters.StaminaTotalAmount);
                if (_currentStamina <= 0)
                {
                    _isDepleted = true;
                }
                else if (_currentStamina >= _movementParameters.StaminaDepletionThreshold)
                {
                    _isDepleted = false;
                }
            }
        }

        public float CurrentSpeed
        {
            get
            {
                return _statesSpeed[_currentState];
            }
        }

        public Vector2 MovementDirection
        {
            get
            {
                return _movementDirection;
            }
        }

        public void SetMovementDirection(Vector2 direction)
        {
            _isMoving = direction.magnitude > 0;

            // does not update rotation if the character is still
            if (_isMoving)
            {
                _movementDirection = direction;

                Vector2 animationDirection = CalculateAnimationDirection(direction);
                _character.AnimationController.SetParameter(AnimationParameters.LookX, animationDirection.x);
                _character.AnimationController.SetParameter(AnimationParameters.LookY, animationDirection.y);
                _character.Attack.SetAttackDirection(animationDirection);
            }
            UpdateMovingState();
        }

        public void SetRunningState(bool isRunningRequested)
        {
            _isRunningRequested = isRunningRequested;
            UpdateMovingState();
        }

        public void Init(CharacterBase characterBase)
        {
            _rigidBody = characterBase.GetComponent<Rigidbody2D>();
            _character = characterBase;

            _currentState = MovingState.Idle;
            _currentStamina = _movementParameters.StaminaTotalAmount;
            _movementDirection = Vector2.zero;
            _isRunningRequested = false;
            _isDepleted = false;
            _isMoving = false;

            _statesSpeed = new Dictionary<MovingState, float>
            {
                { MovingState.Idle, 0f },
                { MovingState.Walking, _movementParameters.MovementSpeed },
                { MovingState.Running, _movementParameters.RunningSpeed }
            };

            _statesStaminaChangeRate = new Dictionary<MovingState, float>
            {
                { MovingState.Idle, _movementParameters.StaminaRegenRate },
                { MovingState.Walking, _movementParameters.StaminaWalkRegenRate },
                { MovingState.Running, -_movementParameters.StaminaDepletionRate }
            };
        }

        public void UpdateComponent(float deltaTime)
        {
            UpdateMovingState();

            _character.AnimationController.SetParameter(AnimationParameters.MovingState, (int)_currentState);
            Stamina += _statesStaminaChangeRate[_currentState] * deltaTime;
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
            _rigidBody.velocity = _statesSpeed[_currentState] * _movementDirection;
        }

        private void UpdateMovingState()
        {
            if (_isRunningRequested && !_isDepleted)
            {
                _currentState = MovingState.Running;
            }
            else if (_isMoving)
            {
                _currentState = MovingState.Walking;
            }
            else
            {
                _currentState = MovingState.Idle;
            }
        }

        private static Vector2 CalculateAnimationDirection(Vector2 movementDirection)
        {
            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
            {
                return new Vector2(Mathf.Sign(movementDirection.x), 0);
            }
            else
            {
                return new Vector2(0, Mathf.Sign(movementDirection.y));
            }
        }
    }
}
