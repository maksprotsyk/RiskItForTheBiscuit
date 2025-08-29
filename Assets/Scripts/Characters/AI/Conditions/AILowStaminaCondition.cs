using UnityEngine;

namespace Characters.AI
{
    public class AILowStaminaCondition: AIStateChangeConditionBase
    {
        [SerializeField] private float _staminaThreshold = 0.1f;

        public override bool IsSatisfied()
        {
            return _character.Movement.StaminaPercentage < _staminaThreshold;
        }
    }

}
