using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public enum AnimationParameters
    {
        Attack,
        Death,
        Hurt,
        MovingState,
        LookX,
        LookY,
        COUNT
    }
    public class CharacterAnimationController
    {
        ///////////////////////////////////////////////////////

        private readonly Animator _animator;
        private readonly List<int> _parameterHashes;

        ///////////////////////////////////////////////////////

        public CharacterAnimationController(Animator i_animator)
        {
            _animator = i_animator;
            _parameterHashes = new();
            for (AnimationParameters parameter = 0; parameter < AnimationParameters.COUNT; parameter++)
            {
                _parameterHashes.Add(Animator.StringToHash(parameter.ToString()));
            }
        }

        ///////////////////////////////////////////////////////
        public void SetAnimationSpeed(float i_speed)
        {
            _animator.speed = i_speed;
        }

        ///////////////////////////////////////////////////////

        public void SetParameter(AnimationParameters i_parameter, bool i_value)
        {
            _animator.SetBool(_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////

        public void SetParameter(AnimationParameters i_parameter, float i_value)
        {
            _animator.SetFloat(_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////
        public void SetParameter(AnimationParameters i_parameter, int i_value)
        {
            _animator.SetInteger(_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////

        public void SetTrigger(AnimationParameters i_parameter)
        {
            _animator.SetTrigger(_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
        public void GetParameter(AnimationParameters i_parameter, out bool i_value)
        {
            i_value = _animator.GetBool(_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////

        public void GetParameter(AnimationParameters i_parameter, out float i_value)
        {
            i_value = _animator.GetFloat(_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
        public void GetParameter(AnimationParameters i_parameter, out int i_value)
        {
            i_value = _animator.GetInteger(_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
    }
}
