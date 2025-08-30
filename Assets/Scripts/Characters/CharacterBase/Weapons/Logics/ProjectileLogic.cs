using System;
using Characters.Combat;
using DataStorage.Generated;
using Items;
using UnityEngine;

namespace Characters.Weapons
{
    [CreateAssetMenu(menuName = "RPG/WeaponLogic/Projectile")]
    public class ProjectileLogic : WeaponLogicBase
    {
        [Header("Options")] public bool SpawnOnAnimEvent = true;

        public override IWeaponInstance CreateInstance(WeaponRuntimeContext ctx) => new Instance(ctx, this);

        private sealed class Instance : IWeaponInstance
        {
            private readonly WeaponRuntimeContext _ctx;
            private readonly WeaponDefinition _def;
            private readonly ProjectileLogic _logic;

            private Vector2 _aim = Vector2.right;

            public Instance(WeaponRuntimeContext ctx, ProjectileLogic logic)
            {
                _ctx = ctx;
                _def = ctx.Definition;
                _logic = logic;
            }

            public void OnEquip()
            {
            }

            public void Dispose()
            {
            }

            public void Tick(float dt)
            {
            }

            public bool CanFire() => _def.ProjectilePrefab;

            public void Fire()
            {
                if (!_logic.SpawnOnAnimEvent) Spawn();
            }

            public void OnAnimationStart()
            {
                if (_logic.SpawnOnAnimEvent)
                    Spawn();
            }

            public void SetAim(Vector2 dir)
            {
                if (dir.sqrMagnitude > 0.0001f) _aim = dir.normalized;
            }

            public float GetEffectiveRange() => Mathf.Max(0f, _def.Range);

            private void Spawn()
            {
                if (!_def.ProjectilePrefab) return;

                var origin = _ctx.Origin.position;
                var go = UnityEngine.Object.Instantiate(_def.ProjectilePrefab, origin, Quaternion.identity);

                float angle = Mathf.Atan2(_aim.y, _aim.x) * Mathf.Rad2Deg;
                go.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

                // Movement parameters (scaled by stats if present)
                var s = _ctx.StatsHub.Stats;
                float speedScale = 1f + s.Get(new StatsDef("ProjectileSpeedPercent"));
                float rangeScale = 1f + s.Get(new StatsDef("ProjectileRangePercent"));

                float speed = Mathf.Max(0f, _def.Speed * speedScale);
                float range = Mathf.Max(0f, _def.Range * rangeScale);

                var mover = go.GetComponent<ProjectileMover>();
                if (!mover) mover = go.AddComponent<ProjectileMover>();
                mover.Init(_aim, speed, range);

                // Damage payload
                var writer = go.GetComponent<IDamageWriter>();
                if (writer != null) writer.SetPayload(BuildPayload(_aim));
            }

            private DamagePayload BuildPayload(Vector2 dir)
            {
                var s = _ctx.StatsHub.Stats;

                float damageStat = s.Get(StatsDef.Damage);
                float dmgAddPercent = s.Get(new StatsDef("DamagePercentAdd"));
                float dmgMultPercent = s.Get(new StatsDef("DamagePercentMult"));

                float baseDmg = _def.Damage + damageStat;
                float dmg = baseDmg * (1f + Mathf.Max(0f, dmgAddPercent));
                dmg *= Mathf.Max(0f, 1f + dmgMultPercent);

                return new DamagePayload(
                    amount: Mathf.Max(0f, dmg),
                    dir,
                    source: _ctx.Owner
                );
            }
        }
    }
}
