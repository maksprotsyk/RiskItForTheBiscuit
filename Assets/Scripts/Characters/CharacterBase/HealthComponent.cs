using System;
using Characters.Stats;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class HealthComponent : ICharacterComponent
    {
        private CharacterAnimationController _animationController;
        private CharacterStatsHub _statsHub;

        private float _currentHealth;
        private float _currentArmor;
        private float _healthRegenTimer;
        private float _invulnerabilityTimer;

        public void Init(CharacterBase characterBase)
        {
            _animationController = characterBase.AnimationController;
            _statsHub = characterBase.GetComponent<CharacterStatsHub>();

            _currentHealth = MaxHealth; // start full
            _currentArmor = Mathf.Min(_currentArmor, MaxArmor);

            _statsHub.Stats.OnStatChanged += OnStatChanged;
        }

        private float MaxHealth => _statsHub.Stats.Get(_statsHub.MaxHealth);
        private float MaxArmor => _statsHub.Stats.Get(_statsHub.MaxArmor);
        private float RegenRate => _statsHub.Stats.Get(_statsHub.HealthRegenRate);
        private float DelayBeforeRegen => _statsHub.Stats.Get(_statsHub.DelayBeforeRegen);
        private float InvulnDuration => _statsHub.Stats.Get(_statsHub.InvulnerabilityDuration);

        public void UpdateComponent(float dt)
        {
            _invulnerabilityTimer = Mathf.Max(0, _invulnerabilityTimer - dt);
            _healthRegenTimer = Mathf.Max(0, _healthRegenTimer - dt);
            if (_healthRegenTimer <= 0f)
                _currentHealth = Mathf.Min(MaxHealth, _currentHealth + RegenRate * dt);
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
        }

        public void TakeDamage(float hp)
        {
            if (IsInvulnerable) return;
            IsInvulnerable = true;

            float armorBefore = _currentArmor;
            _currentArmor = Mathf.Max(0, _currentArmor - hp);
            hp -= (armorBefore - _currentArmor);

            if (hp > 0)
            {
                _currentHealth = Mathf.Max(0, _currentHealth - hp);
                if (_currentHealth <= 0) _animationController.SetTrigger(AnimationParameters.Death);
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

        private bool IsInvulnerable
        {
            get => _invulnerabilityTimer > 0f;
            set => _invulnerabilityTimer = value ? InvulnDuration : 0f;
        }

        private void OnStatChanged(StatDefinition s)
        {
            if (s == _statsHub.MaxHealth)
                _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
            if (s == _statsHub.MaxArmor)
                _currentArmor = Mathf.Min(_currentArmor, MaxArmor);
        }
    }
}
