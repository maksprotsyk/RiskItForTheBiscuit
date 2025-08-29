using UnityEngine;
using Characters.Weapons;

namespace Items
{
    [CreateAssetMenu(menuName = "RPG/Weapon", fileName = "NewWeapon")]
    public class WeaponDefinition : ItemDefinition
    {
        [Header("Behaviour")]
        public WeaponLogicBase Logic;

        [Header("Common numbers")]
        [Min(0f)] public float Damage = 10f;
        [Min(0f)] public float Cooldown = 0.5f;
        [Min(0f)] public float Range = 1.0f;
        [Min(0f)] public float Speed = 10f;

        [Tooltip("Optional generic prefab used by some logics (e.g. melee hitbox or projectile)")]
        public GameObject ProjectilePrefab;

        [Header("Animation")]
        [Tooltip("Name of animation event to spawn/enable. Default: 'HitOn'")]
        public string FireEventName = "HitOn";
        [Tooltip("Name of animation event to disable. Default: 'HitOff'")]
        public string StopEventName = "HitOff";
    }
}