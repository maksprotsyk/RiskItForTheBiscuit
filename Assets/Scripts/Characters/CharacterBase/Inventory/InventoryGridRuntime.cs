using System.Collections.Generic;
using Characters.Effects;
using Characters.Stats;
using Items;
using System;
using UnityEngine;

namespace Characters.Inventory
{
    // Core logic for Inventory is handled by this class
    // * add new item
    // * remove/consume item
    // * apply item buff
    public sealed class InventoryGridRuntime
    {
        public const int Rows = 3;
        public const int Cols = 3;

        private readonly ItemDefinition[,] _grid = new ItemDefinition[Rows, Cols];
        private readonly StatCollection _stats;
        private readonly EffectSystem _effects;
        private readonly CharacterStatsHub _hub; // handy for Orders/constants or weapon hooks

        public event Action OnChanged; // UI can subscribe

        public InventoryGridRuntime(CharacterStatsHub hub)
        {
            _hub = hub;
            _stats = hub.Stats;
            _effects = hub.Effects;
        }

        public bool IsValidSlot(ItemDefinition item, int row, int col)
        {
            if (!item || !InBounds(row, col)) return false;
            if (!Accepts(row, item)) return false;
            if (_grid[row, col] != null) return false;

            return true;
        }

        public ItemDefinition Get(int row, int col) => InBounds(row, col) ? _grid[row, col] : null;

        public bool TryPlace(ItemDefinition item, int row, int col)
        {
            bool CanPlaceHere = IsValidSlot(item, row, col);
            if (!CanPlaceHere)
            {
                return CanPlaceHere;
            }

            _grid[row, col] = item;
            ApplyPersistent(item, apply: true);

            // Optional: if item.Kind == Weapon and top row, you can switch current weapon here.

            OnChanged?.Invoke();
            return true;
        }

        public bool TryMove(int srcRow, int srcCol, int dstRow, int dstCol)
        {
            if (!InBounds(srcRow, srcCol) || !InBounds(dstRow, dstCol)) return false;
            var item = _grid[srcRow, srcCol];
            if (item == null) return false;
            if (!Accepts(dstRow, item)) return false;
            if (_grid[dstRow, dstCol] != null) return false;

            _grid[srcRow, srcCol] = null;
            _grid[dstRow, dstCol] = item;
            // Persistent modifiers don’t change; they stay applied because the item remained in the grid.
            OnChanged?.Invoke();
            return true;
        }

        public bool TryRemove(int row, int col)
        {
            if (!InBounds(row, col)) return false;
            var item = _grid[row, col];
            if (item == null) return false;

            _grid[row, col] = null;
            ApplyPersistent(item, apply: false);
            OnChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// RMB consume: for Passive items adds a timed extra so total power becomes ConsumeTotalMultiplier ×.
        /// Weapons can have custom WIP behavior later.
        /// </summary>
        public bool TryConsume(int row, int col)
        {
            if (!InBounds(row, col)) return false;
            var item = _grid[row, col];
            if (item == null) return false;

            var passiveItem = item as PassiveItemDefinition;
            if (passiveItem)
            {
                float extraFactor = Mathf.Max(0f, passiveItem.ConsumeTotalMultiplier - 1f);
                if (extraFactor <= 0f) return false;

                var extra = BuildScaledModifiers(item, extraFactor, source: item); // same source is okay (EffectSystem uses distinct TimedEffect instances)
                _effects.Add(new TimedEffect(item, extra, passiveItem.ConsumeDurationSeconds));
                return true;
            }

            // Weapon consume behavior (WIP) – stub for later
            // e.g., temporary special attack mode
            return false;
        }

        public IReadOnlyList<(int row, int col, ItemDefinition item)> Snapshot()
        {
            var list = new List<(int, int, ItemDefinition)>(Rows * Cols);
            for (var r = 0; r < Rows; r++)
                for (var c = 0; c < Cols; c++)
                    if (_grid[r, c]) list.Add((r, c, _grid[r, c]));
            return list;
        }

        // -------- internals --------

        private static bool InBounds(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;

        /// <summary>Top row (row 0) allows Weapons; rows 1-2 allow Passives.</summary>
        private static bool Accepts(int row, ItemDefinition item)
        {
            if (row == 0) return item.GetType() == typeof(WeaponDefinition); // row 0
            return item.GetType() == typeof(PassiveItemDefinition); // rows 1 and 2
        }

        private void ApplyPersistent(ItemDefinition item, bool apply)
        {
            // Passive items grant constant buffs while present in grid.
            // Weapons can also have persistent modifiers if you like (e.g., +range while equipped).
            if (item.Modifiers == null) return;

            if (apply)
            {
                foreach (var d in item.Modifiers)
                    _stats.AddModifier(new StatModifier(d.Stat, d.Op, d.Value, item, d.Order));
            }
            else
            {
                _stats.RemoveAllFromSource(item);
            }
        }

        private static List<StatModifier> BuildScaledModifiers(ItemDefinition item, float factor, object source)
        {
            var list = new List<StatModifier>(item.Modifiers?.Length ?? 0);
            if (item.Modifiers == null) return list;

            foreach (var d in item.Modifiers)
            {
                // Scale same way for Flat/PercentAdd/PercentMult/FinalAdd.
                // With factor = 1.0 and base already applied, total becomes 2×.
                list.Add(new StatModifier(d.Stat, d.Op, d.Value * factor, source, d.Order));
            }
            return list;
        }
    }
}