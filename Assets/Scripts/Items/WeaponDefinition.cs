using UnityEngine;
using Characters.Weapons;
using DataStorage.Generated;

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

        [Header("Audio")]
        public SFXAssets AttackSound;
    }
}