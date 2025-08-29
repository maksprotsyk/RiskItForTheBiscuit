using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public interface IAIStateChangeCondition
    {
        public void Init(CharacterBase character);
        public bool IsSatisfied();
        public void OnStateEnter();
        public void OnStateExit();
        public void OnStateUpdate(float deltaTime);
        public bool ShouldReturnTrue { get; }
    }

    [System.Serializable]
    public class AIStateChangeConditionWrapper
    {
        [SerializeReference]
        public IAIStateChangeCondition Condition;
    }

}
