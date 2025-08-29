using Managers;

namespace Characters.AI
{
    public class AICanAttackPlayerCondition: AIStateChangeConditionBase
    {
        private const float AttackRangeCheckDelay = 1.0f;

        private CharacterBase _playerCharacter;
        private float _timeSinceLastCheck = 0f;
        bool _lastResult = false;

        public override void Init(CharacterBase character)
        {
            base.Init(character);
            GameplayManager gameplayManager = ManagersOwner.GetManager<GameplayManager>();
            _playerCharacter = gameplayManager.PlayerController.GetComponent<CharacterBase>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _timeSinceLastCheck = AttackRangeCheckDelay;
        }

        public override void OnStateUpdate(float deltaTime)
        {
            base.OnStateUpdate(deltaTime);
            _timeSinceLastCheck += deltaTime;
            if (_timeSinceLastCheck >= AttackRangeCheckDelay)
            {
                _lastResult = _character.Weapon.IsInRange(_playerCharacter);
                _timeSinceLastCheck = 0f;
            }
        }
        public override bool IsSatisfied()
        {
            return _lastResult;
        }
    }

}
