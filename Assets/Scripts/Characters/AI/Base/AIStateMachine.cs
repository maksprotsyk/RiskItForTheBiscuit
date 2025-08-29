using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    [Serializable]
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

                Debug.Log($"Transitioning from {_state} to {value}");
                _statesLogic[_state].StateLogic.OnExit();
                _state = value;
                _statesLogic[_state].StateLogic.OnEnter();

                //Debug.Log($"Entered state: {m_state}");
            }
        }
        public void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            foreach (AIStateWrapper stateLogic in _statesLogic.Values)
            {
                stateLogic.StateLogic.Init(i_character, i_agent);
            }
            _state = _initialState;
            _statesLogic[_initialState].StateLogic.OnEnter();
        }

        public void Update(float dt)
        {
            _statesLogic[_state].StateLogic.OnUpdate(dt);
            if (_statesLogic[_state].StateLogic.TryGetNextState(out AIState newState))
            {
                State = newState;
            }
        }

    }
}

