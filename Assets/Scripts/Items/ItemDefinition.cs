using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Items
{
    public enum PowerType { Sword, Magic, Sneakers, Common }
    public enum ItemKind { Weapon, Passive }

    [System.Serializable]
    public struct ModifierDescriptor
    {
        public StatsDef Stat;
        public StatOp Op;
        public float Value;
        public int Order; // Flat=100, PercentAdd=200, PercentMult=300, FinalAdd=400 by convention
    }

    [CreateAssetMenu(menuName = "RPG/Item")]
    public class ItemDefinition : ScriptableObject
    {
        [Header("Kind (weapon/passive)")]
        public ItemKind Kind;

        [Header("Power type")]
        public PowerType PowerType;

        [Header("Persistent modifiers (while item is in grid)")]
        public ModifierDescriptor[] Modifiers;

        [Header("Consume behavior (RMB)")]
        [Tooltip("How long the temporary boost lasts when consumed.")]
        public float ConsumeDurationSeconds = 8f;

        [Tooltip("Total power during the consume window, relative to the base effect. 2.0 => total 2× (adds +1× on top of base).")]
        public float ConsumeTotalMultiplier = 2f;

        // Optional: weapon-specific fields (e.g., AttackType, Projectile prefab, etc.) can go here.
    }
}