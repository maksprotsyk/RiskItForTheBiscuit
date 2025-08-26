using System;
using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class MovementComponent : ICharacterComponent
    {
        public enum MovingState
        {
            Idle = 0,
            Walking = 1,
            Running = 2
        }

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
        private bool _isRunningRequested;
        private bool _isDepleted;
        private bool _isMoving;

        public float Stamina { get; private set; }

        public float CurrentSpeed => _currentState switch
        {
            MovingState.Running => _runSpeed,
            MovingState.Walking => _walkSpeed,
            _ => 0f
        };

        public Vector2 MovementDirection { get; private set; }

        public void SetMovementDirection(Vector2 direction)
        {
            _isMoving = direction.sqrMagnitude > 0.0001f;

            if (_isMoving)
            {
                MovementDirection = direction.normalized;

                var animDir = CalculateAnimationDirection(direction);
                _character.AnimationController.SetParameter(AnimationParameters.LookX, animDir.x);
                _character.AnimationController.SetParameter(AnimationParameters.LookY, animDir.y);
                _character.Attack.SetAttackDirection(MovementDirection);
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
            if (!_rigidBody) Debug.LogError("MovementComponent requires Rigidbody2D.", characterBase);

            _statsHub = characterBase.GetComponent<CharacterStatsHub>();
            if (!_statsHub) Debug.LogError("MovementComponent requires CharacterStatsHub.", characterBase);

            RebuildCache();
            _statsHub.Stats.OnStatChanged += OnStatChanged;

            _currentState = MovingState.Idle;
            MovementDirection = Vector2.zero;
            _isRunningRequested = false;
            _isMoving = false;
            _isDepleted = false;

            Stamina = _staminaTotal; // start full
        }

        public void OnDestroy()
        {
            if (_statsHub) _statsHub.Stats.OnStatChanged -= OnStatChanged;
        }

        public void UpdateComponent(float deltaTime)
        {
            UpdateMovingState();

            _character.AnimationController.SetParameter(AnimationParameters.MovingState, (int)_currentState);

            var rate = _currentState switch
            {
                MovingState.Idle => _staminaRegenIdle,
                MovingState.Walking => _staminaRegenWalk,
                MovingState.Running => -_staminaDepleteRun,
                _ => 0f
            };

            Stamina = Mathf.Clamp(Stamina + rate * deltaTime, 0f, _staminaTotal);

            if (Stamina <= 0f)
            {
                _isDepleted = true;
            }
            else if (Stamina >= _staminaDepletionThreshold)
            {
                _isDepleted = false;
            }
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
            if (!_rigidBody) return;
            _rigidBody.velocity = CurrentSpeed * MovementDirection;
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

        private void OnStatChanged(StatsDef stat)
        {
            // react only to relevant movement stats
            if (stat == StatsDef.MoveSpeed ||
                stat == StatsDef.RunSpeed ||
                stat == StatsDef.StaminaTotal ||
                stat == StatsDef.StaminaRegenRate ||
                stat == StatsDef.StaminaWalkRegenRate ||
                stat == StatsDef.StaminaDepletionRate ||
                stat == StatsDef.StaminaDepletionThreshold)
            {
                var prevTotal = _staminaTotal;
                var prevStamina = Stamina;

                RebuildCache();

                // keep stamina ratio when max changes (feels good with buffs)
                if (!Mathf.Approximately(prevTotal, _staminaTotal) && prevTotal > 0f)
                {
                    var ratio = Mathf.Clamp01(prevStamina / prevTotal);
                    Stamina = ratio * _staminaTotal;
                }

                // re-evaluate depletion immediately
                _isDepleted = Stamina <= 0f || (Stamina < _staminaDepletionThreshold && _isDepleted);
            }
        }

        private void RebuildCache()
        {
            _walkSpeed = _statsHub.Stats.Get(StatsDef.MoveSpeed);
            _runSpeed = _statsHub.Stats.Get(StatsDef.RunSpeed);
            _staminaTotal = Mathf.Max(0f, _statsHub.Stats.Get(StatsDef.StaminaTotal));
            _staminaRegenIdle = _statsHub.Stats.Get(StatsDef.StaminaRegenRate);
            _staminaRegenWalk = _statsHub.Stats.Get(StatsDef.StaminaWalkRegenRate);
            _staminaDepleteRun = Mathf.Max(0f, _statsHub.Stats.Get(StatsDef.StaminaDepletionRate));
            _staminaDepletionThreshold = Mathf.Clamp(_statsHub.Stats.Get(StatsDef.StaminaDepletionThreshold), 0f, _staminaTotal);
        }

        private static Vector2 CalculateAnimationDirection(Vector2 movementDirection)
        {
            // 4-dir snap for look/attack
            return Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y) ? 
                new Vector2(Mathf.Sign(movementDirection.x), 0) : new Vector2(0, Mathf.Sign(movementDirection.y));
        }
    }
}
