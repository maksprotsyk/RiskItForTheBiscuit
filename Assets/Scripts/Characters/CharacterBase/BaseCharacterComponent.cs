using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    public abstract class BaseChracterComponent: ICharacterComponent
    {
        protected CharacterBase _character;
        public virtual void Init(CharacterBase characterBase)
        {
            _character = characterBase;
            AddListeners();
        }

        public virtual void OnStart()
        {
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
