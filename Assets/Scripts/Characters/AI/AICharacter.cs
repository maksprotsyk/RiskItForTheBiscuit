using System.Collections;
using System.Collections.Generic;
using Characters.AI;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public class AICharacter : MonoBehaviour
    {

        [SerializeReference] private IAIStateMachine m_stateMachine;
        private CharacterBase m_character;
        private NavMeshAgent m_agent;
        private void Awake()
        {
            m_character = GetComponent<CharacterBase>();
            m_agent = GetComponent<NavMeshAgent>();
            m_agent.updatePosition = false;
            m_agent.updateRotation = false;
        }

        private void Start()
        {
            m_stateMachine.Init(m_character, m_agent);
        }

        private void Update()
        {
            m_stateMachine.Update(Time.deltaTime);

        }
        private void LateUpdate()
        {
            m_agent.nextPosition = transform.position;
        }

        private void Reset()
        {
            m_stateMachine = new AIStateMachine();
        }
    }
}
