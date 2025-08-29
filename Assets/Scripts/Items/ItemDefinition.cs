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
}