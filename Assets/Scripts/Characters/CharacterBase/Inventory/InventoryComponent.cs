using System.Collections.Generic;
using Items;
using System;

namespace Characters.Inventory
{
    // Bridge-class between core inventory logic and UI
    // * used to propagate calls of UI events (drag/drop)
    [Serializable]
    public class InventoryComponent : BaseCharacterComponent
    {
        private InventoryGridRuntime _runtime;

        public event Action OnChanged; // expose to UI if you want to subscribe directly

        
        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);

            _runtime = new InventoryGridRuntime(characterBase.StatsHub);
            _runtime.OnChanged += () => OnChanged?.Invoke();
        }


        // -------- UI entry points --------
        public bool UI_IsValidSlot(ItemDefinition item, int row, int col) => _runtime.IsValidSlot(item, row, col);

        /// <summary>UI calls when dropping an item from the world into a grid cell.</summary>
        public bool UI_TryPlace(ItemDefinition item, int row, int col) => _runtime.TryPlace(item, row, col);

        /// <summary>UI calls when dragging an item from one cell to another.</summary>
        public bool UI_TryMove(int fromRow, int fromCol, int toRow, int toCol) => _runtime.TryMove(fromRow, fromCol, toRow, toCol);

        /// <summary>UI calls when removing an item (e.g., dropping back to world or discarding).</summary>
        public bool UI_TryRemove(int row, int col) => _runtime.TryRemove(row, col);

        /// <summary>UI calls on RMB to consume (timed boost). Returns true if consumed.</summary>
        public bool UI_TryConsume(int row, int col) => _runtime.TryConsume(row, col);

        /// <summary>UI asks for the current grid to render.</summary>
        public IReadOnlyList<(int row, int col, ItemDefinition item)> UI_Snapshot() => _runtime.Snapshot();

        /// <summary>UI helper to read one cell.</summary>
        public ItemDefinition UI_Get(int row, int col) => _runtime.Get(row, col);

    }
}