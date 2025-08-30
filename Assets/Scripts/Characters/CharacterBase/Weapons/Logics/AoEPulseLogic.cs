using System;
using Characters.Combat;
using DataStorage.Generated;
using UnityEngine;
using Items;

namespace Characters.Weapons
{
    [CreateAssetMenu(menuName = "RPG/WeaponLogic/AoE Pulse")]
    public class AoEPulseLogic : WeaponLogicBase
    {
        [Min(0f)] public float Force = 10.0f;
        public LayerMask AffectedLayers;

        public override IWeaponInstance CreateInstance(WeaponRuntimeContext ctx) => new Instance(ctx, this);

        private sealed class Instance : IWeaponInstance
        {
            private readonly WeaponRuntimeContext _ctx;
            private readonly WeaponDefinition _def;
            private readonly AoEPulseLogic _logic;

            private readonly Collider2D[] _hits = new Collider2D[32];
            private Vector2 _aim = Vector2.up;

            public Instance(WeaponRuntimeContext ctx, AoEPulseLogic logic)
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

            public bool CanFire() => true;

            public void Fire()
            {
                /* if not anim-driven, could call DoPulse() here */
            }

            public void OnAnimationStart()
            {
                DoPulse();
            }

            public void SetAim(Vector2 dir)
            {
                if (dir.sqrMagnitude > 0.0001f) _aim = dir.normalized;
            }

            public float GetEffectiveRange() => Mathf.Max(0f, _def.Range);

            private void DoPulse()
            {
                Vector2 origin = _ctx.Owner.transform.position;
                int count = Physics2D.OverlapCircleNonAlloc(origin, _def.Range, _hits, _logic.AffectedLayers);
                if (count <= 0) return;

                var payload = BuildPayload(_aim);

                for (int i = 0; i < count; i++)
                {
                    var col = _hits[i];
                    if (!col) continue;

                    // Push
                    var rb = col.attachedRigidbody;
                    if (rb)
                    {
                        Vector2 dir = ((Vector2)col.transform.position - origin).normalized;
                        rb.AddForce(dir * _logic.Force, ForceMode2D.Impulse);
                    }

                    var health = col.GetComponentInParent<CharacterBase>().Health;
                    if (health != null) health.TakeDamage(payload.Amount);
                }
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
