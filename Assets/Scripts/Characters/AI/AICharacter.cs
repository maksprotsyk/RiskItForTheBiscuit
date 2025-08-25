using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public class AICharacter : MonoBehaviour
    {

        [SerializeReference] private IAIStateMachine _stateMachine;
        private CharacterBase _character;
        private NavMeshAgent _agent;
        private void Awake()
        {
            _character = GetComponent<CharacterBase>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Start()
        {
            _stateMachine.Init(_character, _agent);
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);

        }
        private void LateUpdate()
        {
            _agent.nextPosition = transform.position;
        }

        private void Reset()
        {
            _stateMachine = new AIStateMachine();
        }
    }
}
