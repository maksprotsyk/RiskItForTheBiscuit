using System;
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

            public Instance(WeaponRuntimeContext ctx)
            {
                _ctx = ctx;
                _def = ctx.Definition;
            }

            public void OnEquip()
            {
            }

            public void Dispose()
            {
                if (_activeHitbox) UnityEngine.Object.Destroy(_activeHitbox);
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

            public void OnAnimEvent(string evt)
            {
                string on = string.IsNullOrWhiteSpace(_def.FireEventName) ? "HitOn" : _def.FireEventName;
                string off = string.IsNullOrWhiteSpace(_def.StopEventName) ? "HitOff" : _def.StopEventName;

                if (string.Equals(evt, on, StringComparison.OrdinalIgnoreCase))
                {
                    if (_activeHitbox || !_def.ProjectilePrefab) return;

                    _activeHitbox = UnityEngine.Object.Instantiate(_def.ProjectilePrefab, _ctx.Owner.transform);
                    OrientChild(_activeHitbox.transform, _aim);

                    var writer = _activeHitbox.GetComponent<IDamageWriter>();
                    if (writer != null) writer.SetPayload(BuildPayload(_aim));
                }
                else if (string.Equals(evt, off, StringComparison.OrdinalIgnoreCase))
                {
                    if (_activeHitbox)
                    {
                        UnityEngine.Object.Destroy(_activeHitbox);
                        _activeHitbox = null;
                    }
                }
            }

            public void SetAim(Vector2 dir)
            {
                if (dir.sqrMagnitude > 0.0001f) _aim = dir.normalized;
                if (_activeHitbox) OrientChild(_activeHitbox.transform, _aim);
            }

            public float GetEffectiveRange() => Mathf.Max(0f, _def.Range);

            // ---- helpers ----
            private void OrientChild(Transform t, Vector2 dir)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                t.localPosition = dir * 0.5f; // tweak reach
                t.localRotation = Quaternion.Euler(0, 0, angle - 90f);
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
