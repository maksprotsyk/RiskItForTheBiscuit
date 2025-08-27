using Characters.Stats;
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
        [Header("Weapon-specific")]
        public GameObject ProjectilePrefab;
        [Min(0f)] public float Damage;   // base damage, before modifiers
        [Min(0f)] public float Cooldown; // base cooldown, before modifiers
        [Min(0f)] public float Range;    // max distance projectile can travel
        [Min(0f)] public float Speed;    // projectile speed
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