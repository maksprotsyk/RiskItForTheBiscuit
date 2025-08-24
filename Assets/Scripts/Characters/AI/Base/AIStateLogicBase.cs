using UnityEngine.AI;

namespace Characters.AI
{

    [System.Serializable]
    public abstract class AIStateLogicBase: IAIStateLogic
    {
        protected CharacterBase m_character;
        protected NavMeshAgent m_agent;
        protected AIState m_initialState;

        public virtual void Init(AIState i_initialState, CharacterBase i_character, NavMeshAgent i_agent)
        {
            m_character = i_character;
            m_initialState = i_initialState;
            m_agent = i_agent;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual AIState OnUpdate(float deltaTime)
        {
            return m_initialState;
        }
    }

}
