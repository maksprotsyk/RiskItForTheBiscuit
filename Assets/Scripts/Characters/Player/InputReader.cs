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
        public event Action<bool> RunningState;
        public event Action AttackEvent;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackEvent?.Invoke(); // Trigger attack event
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RunningState?.Invoke(true);
            }
            else if (context.canceled)
            {
                RunningState?.Invoke(false);
            }
        }
    }
}