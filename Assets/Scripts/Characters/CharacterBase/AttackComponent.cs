using System;
using AYellowpaper.SerializedCollections;
using Characters.Stats;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Characters
{
    public enum AttackType
    {
        Sword,
        Fireball
    }

    [Serializable]
    public class AttackComponent : ICharacterComponent
    {
        [SerializeField] 
        private SerializedDictionary<AttackType, GameObject> _attackProjectiles;

        private CharacterBase _character;
        private CharacterStatsHub _statsHub;

        private GameObject _currentProjectile;
        private AttackType _currentAttackType;
        private Vector2 _attackDirection = Vector2.up;

        private float _cooldownTimer;

        // cached stats
        private float _damage;
        private float _cooldown;

        public void Init(CharacterBase characterBase)
        {
            _character = characterBase;
            _statsHub = characterBase.GetComponent<CharacterStatsHub>();
            if (!_statsHub) Debug.LogError("AttackComponent requires CharacterStatsHub.", characterBase);

            RebuildCache();
            _statsHub.Stats.OnStatChanged += OnStatChanged;

            _currentAttackType = AttackType.Sword;
            _cooldownTimer = 0f;
        }

        public void OnDestroy()
        {
            if (_statsHub) _statsHub.Stats.OnStatChanged -= OnStatChanged;
        }

        public void UpdateComponent(float deltaTime)
        {
            _cooldownTimer = Mathf.Max(0f, _cooldownTimer - deltaTime);
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
        }

        public void SetAttackDirection(Vector2 attackDirection)
        {
            // expects unit 4-dir from Movement
            if (attackDirection.sqrMagnitude > 0.0001f)
                _attackDirection = attackDirection.normalized;
        }


        public void PerformAttack()
        {
            if (_cooldownTimer > 0f) return;

            // block double-spawn for melee that keeps a child collider active until anim turns it off
            if (_currentProjectile && _currentAttackType == AttackType.Sword) return;

            _cooldownTimer = _cooldown;
            _character.AnimationController.SetTrigger(AnimationParameters.Attack);
        }

        /// <summary>
        ///     Called from animation events to toggle the hitbox/projectile.
        ///     For Sword: spawn/destroy a child slash collider.
        ///     For Fireball: typically spawn once at the "shoot" frame; ignore the "off" call if it’s a free projectile.
        /// </summary>
        public void SetProjectileState(bool isEnabled)
        {
            if (!isEnabled)
            {
                if (_currentAttackType == AttackType.Sword && _currentProjectile != null)
                {
                    Object.Destroy(_currentProjectile);
                    _currentProjectile = null;
                }

                return;
            }

            if (_attackProjectiles == null || !_attackProjectiles.TryGetValue(_currentAttackType, out var prefab) ||
                prefab == null)
            {
                Debug.LogWarning($"AttackComponent: no projectile prefab for {_currentAttackType}", _character);
                return;
            }

            if (_currentAttackType == AttackType.Sword)
            {
                _currentProjectile = Object.Instantiate(prefab, _character.transform);
                PositionChildHitbox(_currentProjectile.transform, _attackDirection);
                TrySetupDamage(_currentProjectile, _damage);
            }
            else
            {
                var go = Object.Instantiate(prefab, _character.transform.position, Quaternion.identity);
                OrientAndLaunchProjectile(go.transform, _attackDirection);
                TrySetupDamage(go, _damage);
            }
        }

        public void SetAttackType(AttackType type)
        {
            _currentAttackType = type;
        }

        // ---------- internals ----------

        private void RebuildCache()
        {
            _damage = _statsHub.Stats.Get(_statsHub.Damage);
            _cooldown = Mathf.Max(0f, _statsHub.Stats.Get(_statsHub.AttackCooldown));
        }

        private void OnStatChanged(StatDefinition stat)
        {
            if (stat == _statsHub.Damage || stat == _statsHub.AttackCooldown)
                RebuildCache();
        }

        private static void PositionChildHitbox(Transform t, Vector2 dir)
        {
            var maxAxis = Mathf.Max(Mathf.Abs(t.localPosition.x), Mathf.Abs(t.localPosition.y));
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            t.SetLocalPositionAndRotation(maxAxis * dir, Quaternion.Euler(0, 0, angle - 90f));
        }

        private static void OrientAndLaunchProjectile(Transform t, Vector2 dir)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            t.rotation = Quaternion.Euler(0, 0, angle - 90f);

            var rb = t.GetComponent<Rigidbody2D>();
            if (rb)
            {
                var speed = 10f;
                rb.velocity = dir * speed;
            }
        }

        private static void TrySetupDamage(GameObject go, float damage)
        {
            var dmg = go.GetComponent<IDamageWriter>();
            dmg?.SetDamage(damage);
        }
    }

    public interface IDamageWriter
    {
        void SetDamage(float value);
    }
}
