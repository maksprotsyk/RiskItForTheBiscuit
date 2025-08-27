namespace Characters
{
    public abstract class BaseCharacterComponent : ICharacterComponent
    {
        protected CharacterBase _character;

        public virtual void Init(CharacterBase characterBase)
        {
            _character = characterBase;
        }

        public virtual void OnStart()
        {
            AddListeners();
        }

        public virtual void OnDestroy()
        {
            RemoveListeners();
        }

        public virtual void UpdateComponent(float deltaTime)
        {
        }

        public virtual void FixedUpdateComponent(float fixedDeltaTime)
        {
        }

        protected virtual void AddListeners()
        {
        }

        protected virtual void RemoveListeners()
        {
        }
    }
}
