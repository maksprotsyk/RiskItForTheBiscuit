using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Characters
{
    public enum AttackType
    {
        Sword,
        Fireball
    }

    [Serializable]
    public struct AttackParameters
    {
        public float Damage;
        public float AttackCooldown;
    }

    [Serializable]
    public class AttackComponent: ICharacterComponent
    {
        [SerializeField] private AttackParameters _attackParameters;
        [SerializeField] private SerializedDictionary<AttackType, GameObject> _attackProjectiles;

        private CharacterBase _character;
        private GameObject _currentProjectile;
        private AttackType _currentAttackType;
        private Vector2 _attackDirection;

        public void PerformAttack()
        {
            if (_currentProjectile is not null)
            {
                return;
            }
            _character.AnimationController.SetTrigger(AnimationParameters.Attack);
        }

        public void SetAttackDirection(Vector2 attackDirection)
        {
            _attackDirection = attackDirection;
        }

        public void SetProjectileState(bool isEnabled)
        {
            if (!isEnabled)
            {
                if (_currentAttackType == AttackType.Sword)
                {
                    GameObject.Destroy(_currentProjectile);
                    _currentProjectile = null;
                }
                return;
            }

            _currentProjectile = GameObject.Instantiate(_attackProjectiles[_currentAttackType], _character.transform);
            float projectileDistance = Mathf.Max(Mathf.Abs(_currentProjectile.transform.localPosition.x), Mathf.Abs(_currentProjectile.transform.localPosition.y));
            float angle = Mathf.Atan2(_attackDirection.y, _attackDirection.x) * Mathf.Rad2Deg;
            _currentProjectile.transform.SetLocalPositionAndRotation(projectileDistance * _attackDirection, Quaternion.Euler(0, 0, angle - 90));
        }

        public void Init(CharacterBase characterBase)
        {
            _character = characterBase;
            _attackDirection = Vector2.up;
            _currentAttackType = AttackType.Sword;
            _currentProjectile = null;
        }

        public void UpdateComponent(float deltaTime)
        {
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
        }
    }
}
