using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIEscapingState: AIStateLogicBase
    {
        [SerializeField]
        private float _escapeDistance = 10.0f; // distance to maintain from the player while escaping
        
        private GameplayManager _gameplayManager;
        private Transform _playerTransform;

        public override void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerTransform = _gameplayManager.PlayerController.transform;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Vector2 directionFromPlayer = (_playerTransform.position - _character.transform.position);
            if (directionFromPlayer.sqrMagnitude < _escapeDistance)
            {
                _character.Movement.SetLookDirection(directionFromPlayer);
                _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Running);
            }
            else
            {
                _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
        }

    }

}
