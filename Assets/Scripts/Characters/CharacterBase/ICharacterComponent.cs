using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    public interface ICharacterComponent
    {
        public void Init(CharacterBase characterBase);
        public void UpdateComponent(float deltaTime);
    }
}
