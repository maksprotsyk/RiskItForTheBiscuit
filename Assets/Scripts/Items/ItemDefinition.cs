using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Items
{
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
        public ModifierDescriptor[] Modifiers;
        // more fields: icon, rarity, tags, etc.
    }
}