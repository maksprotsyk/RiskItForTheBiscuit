using System.Collections.Generic;
using Items;
using UnityEngine;
using System;

namespace Characters.Inventory
{
    // Bridge-class between core inventory logic and UI
    // * used to propagate calls of UI events (drag/drop)
    [Serializable]
    public class InventoryComponent : ICharacterComponent
    {
        private InventoryGridRuntime _runtime;

        public event Action OnChanged; // expose to UI if you want to subscribe directly
        
        public void Init(CharacterBase characterBase)
        {
            var hub = characterBase.GetComponent<CharacterStatsHub>();
            if (!hub) { Debug.LogError("InventoryComponent requires CharacterStatsHub."); return; }

            _runtime = new InventoryGridRuntime(hub);
            _runtime.OnChanged += () => OnChanged?.Invoke();
        }

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }

        public void UpdateComponent(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void FixedUpdateComponent(float fixedDeltaTime)
        {
            throw new NotImplementedException();
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