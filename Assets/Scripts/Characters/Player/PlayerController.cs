using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterBase))]
    public class PlayerController : MonoBehaviour
    {
        private InputReader m_inputReader;
        private CharacterBase m_character;

        private void Awake()
        {
            m_inputReader = new InputReader();
            m_character = GetComponent<CharacterBase>();

			AddListeners();
        }

        ///////////////////////////////////////////////////////

        private void OnDestroy()
        {
            RemoveListeners();
        }

        ///////////////////////////////////////////////////////

        private void AddListeners()
        {
            RemoveListeners();

            m_inputReader.MoveEvent += m_character.Movement.SetMovementDirection;
            m_inputReader.AttackEvent += m_character.Attack.PerformAttack;
            m_inputReader.RunningState += m_character.Movement.SetRunningState;
        }

        ///////////////////////////////////////////////////////
        
        private void RemoveListeners()
        {
            m_inputReader.MoveEvent -= m_character.Movement.SetMovementDirection;
            m_inputReader.AttackEvent -= m_character.Attack.PerformAttack;
            m_inputReader.RunningState -= m_character.Movement.SetRunningState;
        }
		
		///////////////////////////////////////////////////////

    }
}
