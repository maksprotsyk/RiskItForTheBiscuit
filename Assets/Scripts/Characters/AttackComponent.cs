using System;
using UnityEngine;

namespace Characters
{

    [Serializable]
    public class AttackComponent
    {
        [Serializable]
        public struct AttackParameters
        {
            public float Damage;
            public float AttackCooldown;
        }

        [SerializeField] private AttackParameters _attackParameters; 
        
        private CharacterAnimationController _animationController;

        public void PerformAttack()
        {
            _animationController.SetTrigger(AnimationParameters.Attack);
        }

        public void Init(CharacterAnimationController animationController)
        {
            _animationController = animationController;
        }

        public void UpdateAttackState(float dt)
        {
            
        }
    }
}
