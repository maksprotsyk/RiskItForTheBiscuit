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
        private Vector3 _playerPosition;

        // to avoid recalculating path on every frame
        private float _pathRecalculationTimer = 0.0f;

        public override void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerTransform = _gameplayManager.PlayerController.transform;
            _playerPosition = _playerTransform.position;

            // avoiding all recalculations at the same time 
            RecalculatePath();
            _pathRecalculationTimer = Random.Range(0.0f, PathRecalculationDelay);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _character.Movement.SetPrefferedMovingState(_runWhileChasing ? MovementComponent.MovingState.Running: MovementComponent.MovingState.Walking);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            _pathRecalculationTimer -= deltaTime;

            if (HasArrivedAtDestination() || ShouldChangePath())
            {
                RecalculatePath();
                ResetPathRecalculationTimer();
            }

            UpdateMovementState();
        }

        public override void OnExit()
        {
            base.OnExit();
            _agent.ResetPath();
            _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
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

            _agent.speed = _character.Movement.CurrentSpeed;
            Vector3 targetVelocity = _agent.desiredVelocity.normalized;
            _character.Movement.SetLookDirection(targetVelocity);
        }
    }

}
