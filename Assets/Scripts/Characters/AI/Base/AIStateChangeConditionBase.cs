using UnityEngine;

namespace Characters.AI
{
    public abstract class AIStateChangeConditionBase: IAIStateChangeCondition
    {
        [SerializeField] protected bool _shouldReturnTrue = true;

        protected CharacterBase _character;

        public bool ShouldReturnTrue => _shouldReturnTrue;
        public virtual void Init(CharacterBase character)
        {
            _character = character;
        }

        public virtual void OnStateEnter()
        {

        }
        public virtual void OnStateExit()
        {

        }
        public virtual void OnStateUpdate(float deltaTime)
        {

        }
        public abstract bool IsSatisfied();

    }

}
