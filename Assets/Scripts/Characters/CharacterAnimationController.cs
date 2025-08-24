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

        private readonly Animator m_animator;
        private readonly List<int> m_parameterHashes;

        ///////////////////////////////////////////////////////

        public CharacterAnimationController(Animator i_animator)
        {
            m_animator = i_animator;
            m_parameterHashes = new();
            for (AnimationParameters parameter = 0; parameter < AnimationParameters.COUNT; parameter++)
            {
                m_parameterHashes.Add(Animator.StringToHash(parameter.ToString()));
            }
        }

        ///////////////////////////////////////////////////////
        public void SetAnimationSpeed(float i_speed)
        {
            m_animator.speed = i_speed;
        }

        ///////////////////////////////////////////////////////

        public void SetParameter(AnimationParameters i_parameter, bool i_value)
        {
            m_animator.SetBool(m_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////

        public void SetParameter(AnimationParameters i_parameter, float i_value)
        {
            m_animator.SetFloat(m_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////
        public void SetParameter(AnimationParameters i_parameter, int i_value)
        {
            m_animator.SetInteger(m_parameterHashes[(int)i_parameter], i_value);
        }

        ///////////////////////////////////////////////////////

        public void SetTrigger(AnimationParameters i_parameter)
        {
            m_animator.SetTrigger(m_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
        public void GetParameter(AnimationParameters i_parameter, out bool i_value)
        {
            i_value = m_animator.GetBool(m_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////

        public void GetParameter(AnimationParameters i_parameter, out float i_value)
        {
            i_value = m_animator.GetFloat(m_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
        public void GetParameter(AnimationParameters i_parameter, out int i_value)
        {
            i_value = m_animator.GetInteger(m_parameterHashes[(int)i_parameter]);
        }

        ///////////////////////////////////////////////////////
    }
}
