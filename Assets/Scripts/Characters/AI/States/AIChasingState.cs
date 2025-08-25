using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIChasingState: AIStateLogicBase
    {
        private GameplayManager _gameplayManager;
        private Transform _playerTransform;
        private Vector3 _playerPosition;

        public override void Init(AIState i_initialState, CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_initialState, i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerTransform = _gameplayManager.PlayerController.transform;
            _playerPosition = _playerTransform.position;
        }

        public override void OnEnter()
        {
            // _character.Movement.CurrentSpeed should not be initially 0. TODO: maybe fix MovementComponent
            _character.Movement.SetMovementDirection(Vector2.left);
            _character.Movement.SetRunningState(false);

            _agent.speed = _character.Movement.CurrentSpeed;
            _agent.SetDestination(_playerTransform.position);
        }

        public override AIState OnUpdate(float deltaTime)
        {
            _agent.speed = _character.Movement.CurrentSpeed;

            if ((_playerTransform.position - _playerPosition).magnitude > float.Epsilon)
            {
                _playerPosition = _playerTransform.position;
                _agent.SetDestination(_playerPosition);
            }

            Vector3 targetVelocity = _agent.desiredVelocity.normalized;
            _character.Movement.SetMovementDirection(targetVelocity);
            
            return _initialState;
        }
    }

}
