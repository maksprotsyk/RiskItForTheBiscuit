using System;
using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class MovementComponent : BaseCharacterComponent
    {
        public enum MovingState
        {
            Idle = 0,
            Walking = 1,
            Running = 2
        }

        private Rigidbody2D _rigidBody;

        // cached stat values (rebuilt when any movement stat changes)
        private float _walkSpeed;
        private float _runSpeed;
        private float _staminaTotal;
        private float _staminaRegenIdle;
        private float _staminaRegenWalk;
        private float _staminaDepleteRun;
        private float _staminaDepletionThreshold;

        private MovingState _currentState;
        private MovingState _requestedState;

        private bool _isDepleted;

        public float Stamina { get; private set; }
        
        public float StaminaPercentage => Stamina / _staminaTotal; 

        public float CurrentSpeed => _currentState switch
        {
            MovingState.Running => _runSpeed,
            MovingState.Walking => _walkSpeed,
            _ => 0f
        };

        public Vector2 LookDirection { get; private set; } = Vector2.up;

        public void SetLookDirection(Vector2 direction)
        {
            if (_character.Health.IsDead || direction.sqrMagnitude < float.Epsilon)
            {
                return;
            }

            LookDirection = direction.normalized;

            var animDir = CalculateAnimationDirection(direction);
            _character.AnimationController.SetParameter(AnimationParameters.LookX, animDir.x);
            _character.AnimationController.SetParameter(AnimationParameters.LookY, animDir.y);
            _character.Weapon.SetAimDirection(LookDirection);
        }

        public void SetPrefferedMovingState(MovingState state)
        {
            _requestedState = state;
            UpdateMovingState();
        }

        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);
            _rigidBody = characterBase.GetComponent<Rigidbody2D>();
            if (!_rigidBody) Debug.LogError("MovementComponent requires Rigidbody2D.", characterBase);

            _currentState = MovingState.Idle;
            _requestedState = MovingState.Idle;
        }

        public override void OnStart()
        {
            base.OnStart();

            RebuildCache();

            // needs stats to be ready
            Stamina = _staminaTotal;
        }

        public override void UpdateComponent(float deltaTime)
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

        public override void FixedUpdateComponent(float fixedDeltaTime)
        {
            if (!_rigidBody) return;
            _rigidBody.velocity = CurrentSpeed * LookDirection;
        }
        protected override void AddListeners()
        {
            _character.StatsHub.Stats.OnStatChanged += OnStatChanged;
        }

        protected override void RemoveListeners()
        {
            _character.StatsHub.Stats.OnStatChanged -= OnStatChanged;
        }

        private void UpdateMovingState()
        {
            if (_character.Health.IsDead)
            {
                _currentState = MovingState.Idle;
                return;
            }

            if (_requestedState == MovingState.Running && _isDepleted)
            {
                _currentState = MovingState.Walking;
            }
            else
            {
                _currentState = _requestedState;
            }
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
            _walkSpeed = _character.StatsHub.Stats.Get(StatsDef.MoveSpeed);
            _runSpeed = _character.StatsHub.Stats.Get(StatsDef.RunSpeed);
            _staminaTotal = Mathf.Max(0f, _character.StatsHub.Stats.Get(StatsDef.StaminaTotal));
            _staminaRegenIdle = _character.StatsHub.Stats.Get(StatsDef.StaminaRegenRate);
            _staminaRegenWalk = _character.StatsHub.Stats.Get(StatsDef.StaminaWalkRegenRate);
            _staminaDepleteRun = Mathf.Max(0f, _character.StatsHub.Stats.Get(StatsDef.StaminaDepletionRate));
            _staminaDepletionThreshold = Mathf.Clamp(_character.StatsHub.Stats.Get(StatsDef.StaminaDepletionThreshold), 0f, _staminaTotal);
        }

        private static Vector2 CalculateAnimationDirection(Vector2 movementDirection)
        {
            // 4-dir snap for look/attack
            return Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y) ? 
                new Vector2(Mathf.Sign(movementDirection.x), 0) : new Vector2(0, Mathf.Sign(movementDirection.y));
        }
    }
}
