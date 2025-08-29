using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public interface IAIStateLogic
    {
        public void Init(CharacterBase character, NavMeshAgent agent);
        public void OnEnter();
        public void OnExit();
        public void OnUpdate(float deltaTime);
        public bool TryGetNextState(out AIState nextState);
    }

    [System.Serializable]
    public class AIStateWrapper
    {
        [SerializeReference]
        public IAIStateLogic StateLogic;
    }

}
