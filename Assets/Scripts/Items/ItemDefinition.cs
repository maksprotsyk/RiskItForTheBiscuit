using Characters.Stats;
using Characters.Weapons;
using DataStorage.Generated;
using UnityEngine;

namespace Items
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum ItemKind { Weapon, Passive }

    [System.Serializable]
    public struct ModifierDescriptor
    {
        public StatsDef Stat;
        public StatOp Op;
        public float Value;
        public int Order; // Flat=100, PercentAdd=200, PercentMult=300, FinalAdd=400 by convention
    }

    public abstract class ItemDefinition : ScriptableObject
    {
        [Header("Basic info")]
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
        public ItemRarity Rarity;

        [Tooltip("Persistent modifiers while item is equipped")]
        public ModifierDescriptor[] Modifiers;
    }
    
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

    [CreateAssetMenu(menuName = "RPG/Passive Item", fileName = "NewPassiveItem")]
    public class PassiveItemDefinition : ItemDefinition
    {
        [Header("Consume behavior")]
        [Tooltip("How long the temporary boost lasts when consumed.")]
        [Min(0f)] public float ConsumeDurationSeconds = 8f;

        [Tooltip("Total power during the consume window, relative to the base effect. 2.0 => total 2× (adds +1× on top of base).")]
        [Min(0f)] public float ConsumeTotalMultiplier = 2f;
    }
}