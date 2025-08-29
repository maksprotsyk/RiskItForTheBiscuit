using Managers;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIAttackingState: AIStateLogicBase
    {
        private GameplayManager _gameplayManager;
        private CharacterBase _playerCharacter;

        public override void Init(CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerCharacter = _gameplayManager.PlayerController.GetComponent<CharacterBase>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            _character.Movement.SetLookDirection(_playerCharacter.transform.position - _character.transform.position);
            if (_character.Weapon.AttackIsReady())
            {
                _character.Weapon.PerformAttack();
            }

        }
    }

}
