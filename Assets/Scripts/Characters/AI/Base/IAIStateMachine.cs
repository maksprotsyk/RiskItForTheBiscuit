using UnityEngine;
using UnityEngine.AI;

namespace Characters.AI
{

    public interface IAIStateMachine
    {
        public AIState State { get; set; }
        public void Update(float dt);

        public void Init(CharacterBase character, NavMeshAgent agent);
    }

}