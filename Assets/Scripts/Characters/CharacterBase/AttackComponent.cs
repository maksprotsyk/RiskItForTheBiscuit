using System;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public struct AttackParameters
    {
        public float Damage;
        public float AttackCooldown;
    }

    [Serializable]
    public class AttackComponent: ICharacterComponent
    {
        [SerializeField] private AttackParameters _attackParameters; 
        
        private CharacterAnimationController _animationController;

        public void PerformAttack()
        {
            _animationController.SetTrigger(AnimationParameters.Attack);
        }

        public void Init(CharacterBase characterBase)
        {
            _animationController = characterBase.AnimationController;
        }

        public void UpdateComponent(float deltaTime)
        {
        }
    }
}
