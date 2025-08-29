using UnityEngine;

namespace Characters.AI
{
    public class AILowHealthCondition: AIStateChangeConditionBase
    {
        [SerializeField] private float _healthThreshold = 0.3f;

        public override bool IsSatisfied()
        {
            return _character.Health.HealthPercentage < _healthThreshold;
        }
    }

}
