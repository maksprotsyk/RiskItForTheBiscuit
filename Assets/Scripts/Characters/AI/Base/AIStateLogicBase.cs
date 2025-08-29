using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    [Serializable]
    public abstract class AIStateLogicBase: IAIStateLogic
    {
        [SerializeField] private SerializedDictionary<AIState, AIStateChangeConditionWrapper> _stateChangeConditions = new();

        protected CharacterBase _character;
        protected NavMeshAgent _agent;

        public virtual void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            _character = i_character;
            _agent = i_agent;

            foreach (AIStateChangeConditionWrapper condition in _stateChangeConditions.Values)
            {
                condition.Condition.Init(i_character);
            }
        }

        public virtual void OnEnter()
        {
            foreach (AIStateChangeConditionWrapper condition in _stateChangeConditions.Values)
            {
                condition.Condition.OnStateExit();
            }
        }

        public virtual void OnExit()
        {
            foreach (AIStateChangeConditionWrapper condition in _stateChangeConditions.Values)
            {
                condition.Condition.OnStateExit();
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {
            foreach (AIStateChangeConditionWrapper condition in _stateChangeConditions.Values)
            {
                condition.Condition.OnStateUpdate(deltaTime);
            }
        }

        public bool TryGetNextState(out AIState nextState)
        {
            foreach (var conditionPair in _stateChangeConditions)
            {
                AIStateChangeConditionWrapper conditionWrapper = conditionPair.Value;
                if (conditionWrapper.Condition == null)
                {
                    Debug.LogError($"AI State Machine: Condition for state {conditionPair.Key} is null!");
                    continue;
                }
                if (conditionWrapper.Condition.IsSatisfied() == conditionWrapper.Condition.ShouldReturnTrue)
                {
                    nextState = conditionPair.Key;
                    return true;
                }
            }

            nextState = AIState.Count;
            return false;
        }
    }

}
