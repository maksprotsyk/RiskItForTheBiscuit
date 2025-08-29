using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIChasingState: AIStateLogicBase
    {
        private const float MinDistanceToRecalculatePath = 0.5f;
        private const float PathRecalculationDelay = 1.0f;

        [SerializeField] private bool _runWhileChasing = true;

        private GameplayManager _gameplayManager;
        private Transform _playerTransform;
        private CharacterBase _playerCharacter;
        private Vector3 _playerPosition;

        // to avoid recalculating path on every frame
        private float _pathRecalculationTimer = 0.0f;

        public override void Init(AIState i_initialState, CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_initialState, i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerTransform = _gameplayManager.PlayerController.transform;
            _playerCharacter = _gameplayManager.PlayerController.GetComponent<CharacterBase>();
            _playerPosition = _playerTransform.position;

            // avoiding all recalculations at the same time 
            RecalculatePath();
            _pathRecalculationTimer = Random.Range(0.0f, PathRecalculationDelay);
        }

        public override void OnEnter()
        {
            _character.Movement.SetRunningState(_runWhileChasing);
        }

        public override AIState OnUpdate(float deltaTime)
        {
            _pathRecalculationTimer -= deltaTime;
            
            if (_character.Weapon.IsInRange(_playerCharacter))
            {
                return AIState.Attacking;
            }

            if (HasArrivedAtDestination() || ShouldChangePath())
            {
                RecalculatePath();
                ResetPathRecalculationTimer();
            }

            UpdateMovementState();

            return base.OnUpdate(deltaTime);
        }

        public override void OnExit()
        {
            _agent.ResetPath();
            _character.Movement.SetMovementDirection(Vector3.zero);
        }

        private bool HasArrivedAtDestination()
        {
            return !_agent.pathPending && _agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance;
        }

        private void ResetPathRecalculationTimer()
        {
            _pathRecalculationTimer = PathRecalculationDelay;
        }

        private bool ShouldChangePath()
        {
            return !_agent.pathPending && _pathRecalculationTimer <= 0 && (_agent.destination - _playerTransform.position).magnitude > MinDistanceToRecalculatePath;
        }

        private void RecalculatePath()
        {
            _playerPosition = _playerTransform.position;
            _agent.SetDestination(_playerPosition);
        }

        private void UpdateMovementState()
        {
            if (_agent.pathPending || !_agent.hasPath)
            {
                return;
            }

            // Updating speed of the NavAgent according to state of the character
            _agent.speed = Mathf.Max(0.01f, _character.Movement.CurrentSpeed);
            Vector3 targetVelocity = _agent.desiredVelocity.normalized;
            _character.Movement.SetMovementDirection(targetVelocity);
        }
    }

}
