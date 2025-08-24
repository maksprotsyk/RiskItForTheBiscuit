using UnityEngine;

namespace Characters
{

    public class CharacterBase : MonoBehaviour
    {
        CharacterAnimationController _animationController;

        private void Awake()
        {
            Animator animator = GetComponent<Animator>();
            _animationController = new CharacterAnimationController(animator);
        }

        public void SetMovementDirection(Vector2 direction)
        {
            _animationController.SetParameter(AnimationParameters.LookX, direction.x);
            _animationController.SetParameter(AnimationParameters.LookY, direction.y);
        }

        public void SetMovingState(int movingState)
        {
            _animationController.SetParameter(AnimationParameters.MovingState, movingState);
        }

        public void Attack()
        {
            _animationController.SetTrigger(AnimationParameters.Attack);
        }





    }
}
