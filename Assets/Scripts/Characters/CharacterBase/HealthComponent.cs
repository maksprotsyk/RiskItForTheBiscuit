using System;
using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class HealthComponent : BaseChracterComponent
    {
        public event Action OnDeath;

        private CharacterAnimationController _animationController;

        private float _currentHealth;
        private float _currentArmor;
        private float _healthRegenTimer;
        private float _invulnerabilityTimer;

        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);

            _animationController = characterBase.AnimationController;
            IsDead = false;
        }

        public override void OnStart()
        {
            base.OnStart();

            _currentHealth = MaxHealth; // start full
            _currentArmor = Mathf.Min(_currentArmor, MaxArmor);
        }

        private float MaxHealth => _character.StatsHub.Stats.Get(StatsDef.MaxHealth);
        private float MaxArmor => _character.StatsHub.Stats.Get(StatsDef.MaxArmor);
        private float RegenRate => _character.StatsHub.Stats.Get(StatsDef.HealthRegenRate);
        private float DelayBeforeRegen => _character.StatsHub.Stats.Get(StatsDef.DelayBeforeRegen);
        private float InvulnDuration => _character.StatsHub.Stats.Get(StatsDef.InvulnerabilityDuration);

        public override void UpdateComponent(float dt)
        {
            _invulnerabilityTimer = Mathf.Max(0, _invulnerabilityTimer - dt);
            _healthRegenTimer = Mathf.Max(0, _healthRegenTimer - dt);
            if (_healthRegenTimer <= 0f)
                _currentHealth = Mathf.Min(MaxHealth, _currentHealth + RegenRate * dt);
        }

        public void TakeDamage(float hp)
        {
            if (IsInvulnerable || IsDead) return;
            IsInvulnerable = true;

            float armorBefore = _currentArmor;
            _currentArmor = Mathf.Max(0, _currentArmor - hp);
            hp -= (armorBefore - _currentArmor);

            if (hp > 0)
            {
                _currentHealth = Mathf.Max(0, _currentHealth - hp);
                if (_currentHealth <= 0)
                {
                    _animationController.SetTrigger(AnimationParameters.Death);
                    IsDead = true;
                    OnDeath?.Invoke();
                }
                else
                {
                    _animationController.SetTrigger(AnimationParameters.Hurt);
                    _healthRegenTimer = DelayBeforeRegen;
                }
            }
            else
            {
                _animationController.SetTrigger(AnimationParameters.Hurt);
            }
        }

        public void Heal(float hp) => _currentHealth = Mathf.Min(MaxHealth, _currentHealth + hp);
        public void AddArmor(float a) => _currentArmor = Mathf.Min(MaxArmor, _currentArmor + a);
        public bool IsDead { get; private set; }

        private bool IsInvulnerable
        {
            get => _invulnerabilityTimer > 0f;
            set => _invulnerabilityTimer = value ? InvulnDuration : 0f;
        }

        private void OnStatChanged(StatsDef stat)
        {
            if (stat == StatsDef.MaxHealth)
                _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
            if (stat == StatsDef.MaxArmor)
                _currentArmor = Mathf.Min(_currentArmor, MaxArmor);
        }

        protected override void AddListeners()
        {
            RemoveListeners();

            _character.StatsHub.Stats.OnStatChanged += OnStatChanged;
        }

        protected override void RemoveListeners()
        {
            _character.StatsHub.Stats.OnStatChanged -= OnStatChanged;
        }
    }
}
