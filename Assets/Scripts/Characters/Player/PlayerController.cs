using UnityEngine;

namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {

        private InputReader m_inputReader;
        private CharacterBase m_character;

        private void Awake()
        {
            m_inputReader = new InputReader();
            m_character = GetComponentInChildren<CharacterBase>();

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

            m_inputReader.MoveEvent += SetMovementDirection;
            m_inputReader.AttackStartedEvent += OnAttackStarted;
            m_inputReader.AttackCanceledEvent += OnAttackCanceled;
        }

        ///////////////////////////////////////////////////////
        
        private void SetMovementDirection(Vector2 i_direction)
        {

        }

        ///////////////////////////////////////////////////////

        private void OnAttackStarted()
        {

        }

        ///////////////////////////////////////////////////////
        
        private void OnAttackCanceled()
        {

        }

        ///////////////////////////////////////////////////////
        
        private void RemoveListeners()
        {
            m_inputReader.MoveEvent -= SetMovementDirection;
            m_inputReader.AttackStartedEvent -= OnAttackStarted;
            m_inputReader.AttackCanceledEvent -= OnAttackCanceled;
        }
		
		///////////////////////////////////////////////////////

    }
}
