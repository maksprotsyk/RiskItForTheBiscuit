using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "RPG/Weapon", fileName = "NewWeapon")]
    public class WeaponDefinition : ItemDefinition
    {
        [Header("Weapon-specific")]
        public GameObject ProjectilePrefab;
        [Min(0f)] public float Damage;   // base damage, before modifiers
        [Min(0f)] public float Cooldown; // base cooldown, before modifiers
        [Min(0f)] public float Range;    // max distance projectile can travel
        [Min(0f)] public float Speed;    // projectile speed
    }
}