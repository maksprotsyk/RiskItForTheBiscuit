using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    [System.Serializable]
    public class AIStateMachine: IAIStateMachine
    {
        [SerializeField] protected SerializedDictionary<AIState, AIStateWrapper> m_statesLogic;
        [SerializeField] protected AIState m_initialState;

        protected AIState m_state;

        public AIState State
        {
            get => m_state;
            set
            {
                if (value == m_state)
                {
                    return;
                }

                m_statesLogic[m_state].StateLogic.OnExit();
                m_state = value;
                m_statesLogic[m_state].StateLogic.OnEnter();

                //Debug.Log($"Entered state: {m_state}");
            }
        }
        public void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            foreach (var p in m_statesLogic)
            {
                AIStateWrapper stateLogic = p.Value;
                AIState state = p.Key;
                stateLogic.StateLogic.Init(state, i_character, i_agent);
            }
            m_state = m_initialState;
            m_statesLogic[m_initialState].StateLogic.OnEnter();
        }

        public void Update(float dt)
        {
            AIState newState = m_statesLogic[m_state].StateLogic.OnUpdate(dt);
            State = newState;
        }

    }
}

