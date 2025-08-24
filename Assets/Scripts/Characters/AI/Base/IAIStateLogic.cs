using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public interface IAIStateLogic
    {
        public void Init(AIState i_initialState, CharacterBase character, NavMeshAgent agent);
        public void OnEnter();
        public void OnExit();
        public AIState OnUpdate(float deltaTime);
    }

    [System.Serializable]
    public class AIStateWrapper
    {
        [SerializeReference]
        public IAIStateLogic StateLogic;
    }

}
