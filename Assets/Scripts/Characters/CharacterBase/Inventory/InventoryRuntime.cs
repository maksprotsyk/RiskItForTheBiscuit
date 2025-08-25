using System.Collections.Generic;
using Characters.Stats;
using Items;

namespace Characters.Inventory
{
    public class InventoryRuntime
    {
        private readonly StatCollection _stats;
        private readonly HashSet<ItemDefinition> _equipped = new();

        public InventoryRuntime(StatCollection stats) => _stats = stats;

        public void Equip(ItemDefinition item)
        {
            if (!_equipped.Add(item)) return;
            foreach (var d in item.Modifiers)
                _stats.AddModifier(new StatModifier(d.Stat, d.Op, d.Value, item, d.Order));
        }

        public void Unequip(ItemDefinition item)
        {
            if (_equipped.Remove(item))
                _stats.RemoveAllFromSource(item);
        }
    }
}