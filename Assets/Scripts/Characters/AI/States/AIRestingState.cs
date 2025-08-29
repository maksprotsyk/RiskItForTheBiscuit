using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIRestingState: AIStateLogicBase
    {
        private Transform _playerTransform;
        public override void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_character, i_agent);
            _playerTransform = ManagersOwner.GetManager<GameplayManager>().PlayerController.transform;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            Vector2 directionToPlayer = (_playerTransform.position - _character.transform.position).normalized;
            _character.Movement.SetLookDirection(directionToPlayer);
        }

    }

}
