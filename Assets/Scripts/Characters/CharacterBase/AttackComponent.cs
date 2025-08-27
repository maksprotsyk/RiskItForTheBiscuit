using System;
using AYellowpaper.SerializedCollections;
using Characters.Stats;
using DataStorage.Generated;
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
    public class AttackComponent : BaseChracterComponent
    {
        [SerializeField] 
        private SerializedDictionary<AttackType, GameObject> _attackProjectiles;

        private Collider2D _characterCollider;

        private GameObject _currentProjectile;
        private AttackType _currentAttackType;
        private Vector2 _attackDirection = Vector2.up;
        private Vector2 _attackBaseOffset;

        private float _cooldownTimer;

        // cached stats
        private float _damage;
        private float _cooldown;

        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);

            _characterCollider = characterBase.GetComponent<Collider2D>();
            _currentAttackType = AttackType.Sword;
            _cooldownTimer = 0f;

            SpriteRenderer spriteRenderer = characterBase.GetComponent<SpriteRenderer>();
            _attackBaseOffset = spriteRenderer.bounds.center - characterBase.transform.position;
        }

        public override void OnStart()
        {
            RebuildCache();
        }

        public override void UpdateComponent(float deltaTime)
        {
            _cooldownTimer = Mathf.Max(0f, _cooldownTimer - deltaTime);
        }

        public void SetAttackDirection(Vector2 attackDirection)
        {
            // expects unit 4-dir from Movement
            if (attackDirection.sqrMagnitude > 0.0001f)
                _attackDirection = attackDirection.normalized;
        }

        public bool AttackIsReady()
        {
            if (_cooldownTimer > 0f) return false;

            // block double-spawn for melee that keeps a child collider active until anim turns it off
            if (_currentProjectile && _currentAttackType == AttackType.Sword) return false;

            return true;
        }

        public void PerformAttack()
        {
            if (!AttackIsReady())
            {
                return;
            }

            _cooldownTimer = _cooldown;
            _character.AnimationController.SetTrigger(AnimationParameters.Attack);
        }

        public bool IsInAttackRange(CharacterBase otherCharacter)
        {
            Vector2 selfClosestPoint = _characterCollider.ClosestPoint(otherCharacter.transform.position);
            Vector2 otherClosestPoint = otherCharacter.Attack._characterCollider.ClosestPoint(_character.transform.position);
            Vector2 toOther = otherClosestPoint - selfClosestPoint;

            // TODO: get attack distance from weapon
            float attackDistance = 0.4f;
            return toOther.sqrMagnitude < attackDistance;
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
                PositionChildHitbox(_currentProjectile.transform, _attackDirection, _attackBaseOffset);
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
        protected override void AddListeners()
        {
            _character.StatsHub.Stats.OnStatChanged += OnStatChanged;
        }

        protected override void RemoveListeners()
        {
            _character.StatsHub.Stats.OnStatChanged -= OnStatChanged;
        }

        private void RebuildCache()
        {
            _damage = _character.StatsHub.Stats.Get(StatsDef.Damage);
            _cooldown = Mathf.Max(0f, _character.StatsHub.Stats.Get(StatsDef.AttackCooldown));
        }

        private void OnStatChanged(StatsDef stat)
        {
            if (stat == StatsDef.Damage || stat == StatsDef.AttackCooldown)
                RebuildCache();
        }

        private static void PositionChildHitbox(Transform t, Vector2 dir, Vector2 baseOffset)
        {
            var maxAxis = Mathf.Max(Mathf.Abs(t.localPosition.x), Mathf.Abs(t.localPosition.y));
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            t.SetLocalPositionAndRotation(maxAxis * dir + baseOffset, Quaternion.Euler(0, 0, angle - 90f));
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
