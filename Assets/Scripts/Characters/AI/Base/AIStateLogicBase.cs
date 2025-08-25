using UnityEngine.AI;

namespace Characters.AI
{

    [System.Serializable]
    public abstract class AIStateLogicBase: IAIStateLogic
    {
        protected CharacterBase _character;
        protected NavMeshAgent _agent;
        protected AIState _initialState;

        public virtual void Init(AIState i_initialState, CharacterBase i_character, NavMeshAgent i_agent)
        {
            _character = i_character;
            _initialState = i_initialState;
            _agent = i_agent;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual AIState OnUpdate(float deltaTime)
        {
            return _initialState;
        }
    }

}
