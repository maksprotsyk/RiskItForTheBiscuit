using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    [System.Serializable]
    public class AIStateMachine: IAIStateMachine
    {
        [SerializeField] protected SerializedDictionary<AIState, AIStateWrapper> _statesLogic;
        [SerializeField] protected AIState _initialState;

        protected AIState _state;

        public AIState State
        {
            get => _state;
            set
            {
                if (value == _state)
                {
                    return;
                }

                _statesLogic[_state].StateLogic.OnExit();
                _state = value;
                _statesLogic[_state].StateLogic.OnEnter();

                //Debug.Log($"Entered state: {m_state}");
            }
        }
        public void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            foreach (var p in _statesLogic)
            {
                AIStateWrapper stateLogic = p.Value;
                AIState state = p.Key;
                stateLogic.StateLogic.Init(state, i_character, i_agent);
            }
            _state = _initialState;
            _statesLogic[_initialState].StateLogic.OnEnter();
        }

        public void Update(float dt)
        {
            AIState newState = _statesLogic[_state].StateLogic.OnUpdate(dt);
            State = newState;
        }

    }
}

