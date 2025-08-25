using System;
using Characters.Stats;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class MovementComponent : ICharacterComponent
    {
        public enum MovingState { Idle = 0, Walking = 1, Running = 2 }

        private Rigidbody2D _rigidBody;
        private CharacterBase _character;
        private CharacterStatsHub _statsHub;

        // cached stat values (rebuilt when any movement stat changes)
        private float _walkSpeed;
        private float _runSpeed;
        private float _staminaTotal;
        private float _staminaRegenIdle;
        private float _staminaRegenWalk;
        private float _staminaDepleteRun;
        private float _staminaDepletionThreshold;

        private MovingState _currentState;
        private Vector2 _movementDirection;
        private float _currentStamina;
        private bool _isRunningRequested;
        private bool _isDepleted;
        private bool _isMoving;

        public float Stamina => _currentStamina;
        public float CurrentSpeed => _currentState == MovingState.Running ? _runSpeed :
                                     _currentState == MovingState.Walking ? _walkSpeed : 0f;
        public Vector2 MovementDirection => _movementDirection;

        public void SetMovementDirection(Vector2 direction)
        {
            _isMoving = direction.sqrMagnitude > 0.0001f;

            if (_isMoving)
            {
                _movementDirection = direction;

                Vector2 animDir = CalculateAnimationDirection(direction);
                _character.AnimationController.SetParameter(AnimationParameters.LookX, animDir.x);
                _character.AnimationController.SetParameter(AnimationParameters.LookY, animDir.y);

                // Keep attack facing in sync
                _character.Attack.SetAttackDirection(animDir);
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
            _character = characterBase;
            _rigidBody = characterBase.GetComponent<Rigidbody2D>();
            if (_rigidBody == null) Debug.LogError("MovementComponent requires Rigidbody2D.", characterBase);

            _statsHub = characterBase.GetComponent<CharacterStatsHub>();
            if (_statsHub == null) Debug.LogError("MovementComponent requires CharacterStatsHub.", characterBase);

            RebuildCache();
            _statsHub.Stats.OnStatChanged += OnStatChanged;

            _currentState = MovingState.Idle;
            _movementDirection = Vector2.zero;
            _isRunningRequested = false;
            _isMoving = false;
            _isDepleted = false;

            _currentStamina = _staminaTotal; // start full
        }

        public void UpdateComponent(float deltaTime)
        {
            UpdateMovingState();

            _character.AnimationController.SetParameter(AnimationParameters.MovingState, (int)_currentState);

            float rate = _currentState switch
            {
                MovingState.Idle    => _staminaRegenIdle,
                MovingState.Walking => _staminaRegenWalk,
                MovingState.Running => -_staminaDepleteRun,
                _ => 0f
            };

            _currentStamina = Mathf.Clamp(_currentStamina + rate * deltaTime, 0f, _staminaTotal);

            // depletion gates running
            if (_currentStamina <= 0f) _isDepleted = true;
            else if (_currentStamina >= _staminaDepletionThreshold) _isDepleted = false;
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
            if (_rigidBody == null) return;
            _rigidBody.velocity = CurrentSpeed * _movementDirection;
        }

        private void UpdateMovingState()
        {
            if (_isRunningRequested && !_isDepleted && _isMoving)
                _currentState = MovingState.Running;
            else if (_isMoving)
                _currentState = MovingState.Walking;
            else
                _currentState = MovingState.Idle;
        }

        private void OnStatChanged(StatDefinition stat)
        {
            // react only to relevant movement stats
            if (stat == _statsHub.MoveSpeed ||
                stat == _statsHub.RunSpeed ||
                stat == _statsHub.StaminaTotal ||
                stat == _statsHub.StaminaRegenRate ||
                stat == _statsHub.StaminaWalkRegenRate ||
                stat == _statsHub.StaminaDepletionRate ||
                stat == _statsHub.StaminaDepletionThreshold)
            {
                float prevTotal = _staminaTotal;
                float prevStamina = _currentStamina;

                RebuildCache();

                // keep stamina ratio when max changes (feels good with buffs)
                if (!Mathf.Approximately(prevTotal, _staminaTotal) && prevTotal > 0f)
                {
                    float ratio = Mathf.Clamp01(prevStamina / prevTotal);
                    _currentStamina = ratio * _staminaTotal;
                }

                // re-evaluate depletion immediately
                _isDepleted = _currentStamina <= 0f
                           || (_currentStamina < _staminaDepletionThreshold && _isDepleted);
            }
        }

        private void RebuildCache()
        {
            _walkSpeed                 = _statsHub.Stats.Get(_statsHub.MoveSpeed);
            _runSpeed                  = _statsHub.Stats.Get(_statsHub.RunSpeed);
            _staminaTotal              = Mathf.Max(0f, _statsHub.Stats.Get(_statsHub.StaminaTotal));
            _staminaRegenIdle          = _statsHub.Stats.Get(_statsHub.StaminaRegenRate);
            _staminaRegenWalk          = _statsHub.Stats.Get(_statsHub.StaminaWalkRegenRate);
            _staminaDepleteRun         = Mathf.Max(0f, _statsHub.Stats.Get(_statsHub.StaminaDepletionRate));
            _staminaDepletionThreshold = Mathf.Clamp(_statsHub.Stats.Get(_statsHub.StaminaDepletionThreshold), 0f, _staminaTotal);
        }

        private static Vector2 CalculateAnimationDirection(Vector2 movementDirection)
        {
            // 4-dir snap for look/attack
            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
                return new Vector2(Mathf.Sign(movementDirection.x), 0);
            else
                return new Vector2(0, Mathf.Sign(movementDirection.y));
        }
    }
}
