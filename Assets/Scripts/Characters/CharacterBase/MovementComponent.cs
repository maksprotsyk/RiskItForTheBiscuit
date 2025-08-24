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
        private CharacterAnimationController _animationController;

        private Dictionary<MovingState, float> _statesSpeed;
        private Dictionary<MovingState, float> _statesStaminaChangeRate;

        private Vector2 _movementDirection;
        private float _currentStamina;
        private bool _isRunningRequested;
        private bool _isDepleted;

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

        public void SetMovementDirection(Vector2 direction)
        {
            _movementDirection = direction;

            // does not update rotation if the character is still
            if (_movementDirection.magnitude > 0)
            {
                _animationController.SetParameter(AnimationParameters.LookX, direction.x);
                _animationController.SetParameter(AnimationParameters.LookY, direction.y);
            }
        }

        public void SetRunningState(bool isRunningRequested)
        {
            _isRunningRequested = isRunningRequested;
        }

        public void Init(CharacterBase characterBase)
        {
            _rigidBody = characterBase.GetComponent<Rigidbody2D>();
            _animationController = characterBase.AnimationController;

            _currentStamina = _movementParameters.StaminaTotalAmount;
            _movementDirection = Vector2.zero;
            _isRunningRequested = false;
            _isDepleted = false;

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
            MovingState movingState;

            bool isStanding = _movementDirection.magnitude <= 0;
            if (_isRunningRequested && !_isDepleted && !isStanding)
            {
                movingState = MovingState.Running;
            }
            else if (!isStanding)
            {
                movingState = MovingState.Walking;
            }
            else
            {
                movingState = MovingState.Idle;
            }

            _animationController.SetParameter(AnimationParameters.MovingState, (int)movingState);
            _rigidBody.velocity = _statesSpeed[movingState] * _movementDirection;
            Stamina += _statesStaminaChangeRate[movingState] * deltaTime;
        }
    }
}
