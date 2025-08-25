using System;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public struct HealthParameters
    {
        public float MaxHealth;
        public float MaxArmor;
        public float DelayBeforeRegen;
        public float HealthRegenRate;
        public float InvulnerabilityDuration;
    }

    [Serializable]
    public class HealthComponent: ICharacterComponent
    {

        [SerializeField] private HealthParameters _healthParameters; 
        
        private CharacterAnimationController _animationController;

        private float _currentHealth;
        private float _currentArmor;
        private float _healthRegenTimer;
        private float _invulnerabilityTimer;

        public float CurrentHealth
        {
            get { return _currentHealth; }
            private set { _currentHealth = Mathf.Clamp(value, 0, _healthParameters.MaxHealth); }
        }

        public float CurrentArmor
        {
            get { return _currentArmor; }
            private set { _currentArmor = Mathf.Clamp(value, 0, _healthParameters.MaxArmor); }
        }

        public void TakeDamage(float hitpoints)
        {
            if (IsInvulnerable)
            {
                return;
            }

            IsInvulnerable = true;

            float oldArmor = CurrentArmor;
            CurrentArmor -= hitpoints;
            hitpoints -= oldArmor - CurrentArmor;

            if (CurrentArmor > hitpoints) 
            {
                CurrentArmor -= hitpoints;
                _animationController.SetTrigger(AnimationParameters.Hurt);
                return;
            }

            CurrentHealth -= hitpoints;
            if (CurrentHealth <= 0)
            {
                _animationController.SetTrigger(AnimationParameters.Death);
            }
            else
            {
                _animationController.SetTrigger(AnimationParameters.Hurt);
                _healthRegenTimer = _healthParameters.DelayBeforeRegen;
            }
        }

        public void Heal(float hitpoints)
        {
            CurrentHealth += hitpoints;
        }

        public void AddArmor(float armor)
        {
            CurrentArmor += armor;
        }

        public void Init(CharacterBase characterBase)
        {
            _animationController = characterBase.AnimationController;
        }

        public void UpdateComponent(float deltaTime)
        {
            _invulnerabilityTimer = Mathf.Max(0, _invulnerabilityTimer - deltaTime);
            _healthRegenTimer = Mathf.Max(0, _healthRegenTimer - deltaTime);

            if (_healthRegenTimer <= 0)
            {
                CurrentHealth += _healthParameters.HealthRegenRate * deltaTime;
            }
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
        }

        private bool IsInvulnerable
        {
            get 
            { 
                return _invulnerabilityTimer > 0;
            }
            set
            {
                _invulnerabilityTimer = value ? _healthParameters.InvulnerabilityDuration: 0;
            }
        }
    }
}
