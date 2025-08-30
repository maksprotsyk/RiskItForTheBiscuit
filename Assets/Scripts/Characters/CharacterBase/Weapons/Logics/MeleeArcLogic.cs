using Characters.Combat;
using DataStorage.Generated;
using Items;
using UnityEngine;

namespace Characters.Weapons
{
    [CreateAssetMenu(menuName = "RPG/WeaponLogic/MeleeArc")]
    public class MeleeArcLogic : WeaponLogicBase
    {
        public override IWeaponInstance CreateInstance(WeaponRuntimeContext ctx) => new Instance(ctx);

        private sealed class Instance : IWeaponInstance
        {
            private readonly WeaponRuntimeContext _ctx;
            private readonly WeaponDefinition _def;

            private GameObject _activeHitbox;
            private Vector2 _aim = Vector2.up;
            private float _projectileSpawnDistance;

            public Instance(WeaponRuntimeContext ctx)
            {
                _ctx = ctx;
                _def = ctx.Definition;
                if (_def.ProjectilePrefab == null)
                {
                    Debug.LogWarning("MeleeArcLogic: missing ProjectilePrefab", ctx.Owner);
                    return;
                }

                Vector2 projectileLocalPosition = _def.ProjectilePrefab?.transform?.position ?? Vector2.zero;
                _projectileSpawnDistance = Mathf.Max(Mathf.Abs(projectileLocalPosition.x), Mathf.Abs(projectileLocalPosition.y));
            }

            public void OnEquip()
            {
            }

            public void Dispose()
            {
                if (_activeHitbox) Object.Destroy(_activeHitbox);
                _activeHitbox = null;
            }

            public void Tick(float dt)
            {
            }

            public bool CanFire() => true; // controller handles cooldown

            public void Fire()
            {
                /* animation-driven; spawn on HitOn */
            }

            public void OnAnimationStart()
            {
                if (_activeHitbox || !_def.ProjectilePrefab) return;

                _activeHitbox = Object.Instantiate(_def.ProjectilePrefab, _ctx.Owner.transform);
                PositionChild(_activeHitbox.transform, _aim, _projectileSpawnDistance, _ctx.ProjectileBaseOffset);

                var writer = _activeHitbox.GetComponent<IDamageWriter>();
                if (writer != null) writer.SetPayload(BuildPayload(_aim));
            }

            public void SetAim(Vector2 dir)
            {
                if (dir.sqrMagnitude > 0.0001f) _aim = dir.normalized;
                if (_activeHitbox) PositionChild(_activeHitbox.transform, _aim, _projectileSpawnDistance, _ctx.ProjectileBaseOffset);
            }

            public float GetEffectiveRange() => Mathf.Max(0f, _def.Range);

            // ---- helpers ----
            private void PositionChild(Transform t, Vector2 dir, float projectileSpawnDistance, Vector2 ownerColliderOffset)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                t.SetLocalPositionAndRotation(dir * projectileSpawnDistance + ownerColliderOffset, Quaternion.Euler(0, 0, angle - 90f));
            }

            private DamagePayload BuildPayload(Vector2 dir)
            {
                var s = _ctx.StatsHub.Stats;
                // Safe: unknown stats return 0 by your StatCollection.Get
                float damageStat = s.Get(StatsDef.Damage); // your existing “Damage”
                float dmgAddPercent = s.Get(new StatsDef("DamagePercentAdd"));
                float dmgMultPercent = s.Get(new StatsDef("DamagePercentMult"));

                float baseDmg = _def.Damage + damageStat;
                float dmg = baseDmg * (1f + Mathf.Max(0f, dmgAddPercent));
                dmg *= Mathf.Max(0f, 1f + dmgMultPercent); // convert % to factor

                return new DamagePayload(
                    amount: Mathf.Max(0f, dmg),
                    dir,
                    source: _ctx.Owner
                );
            }
        }
    }
}
