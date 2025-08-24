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

            m_inputReader.MoveEvent += SetMovementDirection;
            m_inputReader.AttackStartedEvent += OnAttackStarted;
            m_inputReader.AttackCanceledEvent += OnAttackCanceled;
        }

        ///////////////////////////////////////////////////////
        
        private void SetMovementDirection(Vector2 i_direction)
        {
            m_character.SetMovementDirection(i_direction);
        }

        ///////////////////////////////////////////////////////

        private void OnAttackStarted()
        {
            m_character.Attack();
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
