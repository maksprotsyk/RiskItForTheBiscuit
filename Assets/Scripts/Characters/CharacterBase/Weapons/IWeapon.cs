using System;
using Items;
using UnityEngine;

namespace Characters.Weapons
{
    public interface IWeaponInstance : IDisposable
    {
        // Called once after creation and before first use
        void OnEquip();

        // Per-frame updates
        void Tick(float dt);

        // Input request to attack; may trigger animation inside the controller instead
        bool CanFire();

        // Request to fire; may be ignored if CanFire() is false
        void Fire();

        // Animation events from animator or CharacterBase hooks
        void OnAnimationStart();

        // Update current aim; weapon may re-orient projectiles/arc
        void SetAim(Vector2 dir);
        
        // Optional: return effective range for AI or UI purposes
        float GetEffectiveRange();
    }

    public readonly struct WeaponRuntimeContext
    {
        public readonly CharacterBase Owner;
        public readonly WeaponDefinition Definition;
        public readonly CharacterStatsHub StatsHub;
        public readonly Transform Origin; // usually Owner.transform
        public readonly Func<Vector2> AimProvider; // returns normalized aim dir
        public readonly Vector2 ProjectileBaseOffset;

        public WeaponRuntimeContext(CharacterBase owner, WeaponDefinition def, CharacterStatsHub hub, Transform origin, Vector2 projectileBaseOffset,
            Func<Vector2> aimProvider)
        {
            Owner = owner;
            Definition = def;
            StatsHub = hub;
            Origin = origin;
            AimProvider = aimProvider;
            ProjectileBaseOffset = projectileBaseOffset;
        }
    }
}