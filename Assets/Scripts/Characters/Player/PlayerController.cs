using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(CharacterBase))]
    public class PlayerController : MonoBehaviour
    {
        private InputReader _inputReader;
        private CharacterBase _character;

        private void Awake()
        {
            _inputReader = new InputReader();
            _character = GetComponent<CharacterBase>();

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

            _inputReader.MoveEvent += _character.Movement.SetMovementDirection;
            _inputReader.AttackEvent += _character.Attack.PerformAttack;
            _inputReader.RunningState += _character.Movement.SetRunningState;
        }

        ///////////////////////////////////////////////////////
        
        private void RemoveListeners()
        {
            _inputReader.MoveEvent -= _character.Movement.SetMovementDirection;
            _inputReader.AttackEvent -= _character.Attack.PerformAttack;
            _inputReader.RunningState -= _character.Movement.SetRunningState;
        }
		
		///////////////////////////////////////////////////////

    }
}
