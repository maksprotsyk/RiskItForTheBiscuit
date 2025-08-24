using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player
{
    public class InputReader : GameInput.IGameplayActions
    {
        private GameInput _gameInput;

        public InputReader()
        {
            _gameInput = new GameInput();
            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.Gameplay.Enable();
        }

        // Gameplay events
        public event Action<Vector2> MoveEvent;
        public event Action AttackStartedEvent;
        public event Action AttackCanceledEvent;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackStartedEvent?.Invoke(); // Start firing when the button is pressed
            }
            else if (context.canceled)
            {
                AttackCanceledEvent?.Invoke(); // Stop firing when the button is released
            }
        }
    }
}