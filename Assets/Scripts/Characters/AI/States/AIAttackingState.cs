using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{
    public class AIAttackingState: AIStateLogicBase
    {
        private const float AttackRangeCheckDelay = 1.0f;

        private GameplayManager _gameplayManager;
        private CharacterBase _playerCharacter;

        private float _attackRangeCheckTimer = 0.0f;

        public override void Init(AIState i_initialState, CharacterBase i_character, NavMeshAgent i_agent)
        {
            base.Init(i_initialState, i_character, i_agent);

            _gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerCharacter = _gameplayManager.PlayerController.GetComponent<CharacterBase>();
        }

        public override void OnEnter()
        {
            _attackRangeCheckTimer = Random.Range(0.0f, AttackRangeCheckDelay);
            _character.Movement.SetPrefferedMovingState(MovementComponent.MovingState.Idle);
        }

        public override AIState OnUpdate(float deltaTime)
        {
            _attackRangeCheckTimer -= deltaTime;
            _character.Movement.SetLookDirection(_playerCharacter.transform.position - _character.transform.position);

            if (_attackRangeCheckTimer <= 0 || _character.Weapon.AttackIsReady())
            {
                ResetAttackRangeCheckTimer();
                if (!_character.Weapon.IsInRange(_playerCharacter))
                {
                    return AIState.Chasing;
                }
                _character.Weapon.PerformAttack();
            }

            return base.OnUpdate(deltaTime);
        }

        private void ResetAttackRangeCheckTimer()
        {
            _attackRangeCheckTimer = AttackRangeCheckDelay;
        }
    }

}
