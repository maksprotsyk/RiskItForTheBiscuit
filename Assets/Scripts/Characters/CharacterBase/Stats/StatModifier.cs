using DataStorage.Generated;

namespace Characters.Stats
{
    public enum StatOp { Flat, PercentAdd, PercentMult, FinalAdd }

    public readonly struct StatModifier
    {
        public readonly StatsDef Stat;
        public readonly StatOp Op;
        public readonly float Value;
        public readonly object Source;   // item/buff/weapon/etc
        public readonly int Order;       // order ties -> sorted, e.g., Flat=100, PercentAdd=200,...

        public StatModifier(StatsDef stat, StatOp op, float value, object source, int order)
        {
            Stat = stat; Op = op; Value = value; Source = source; Order = order;
        }
    }
}