using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    public interface ICharacterComponent
    {
        public void Init(CharacterBase characterBase);
        public void OnStart();
        public void OnDestroy();
        public void UpdateComponent(float deltaTime);
        public void FixedUpdateComponent(float fixedDeltaTime);
    }
}
