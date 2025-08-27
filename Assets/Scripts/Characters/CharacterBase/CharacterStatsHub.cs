using Characters.Effects;
using Characters.Stats;
using UnityEngine;
using DataStorage;
using DataStorage.Generated;
using AYellowpaper.SerializedCollections;
using System;
using Characters.Inventory;

namespace Characters
{
    [Serializable]
    public class CharacterStatsHub : BaseCharacterComponent
    {
        [SerializeField] private IDataContainer<StatsTableRow> _statsDefinitionsTable;

        // set default values
        [SerializeField] private SerializedDictionary<StatsDef, float> _baseStatsOverrides = new()
        {
            { StatsDef.MaxHealth, 100f },
            { StatsDef.MaxArmor, 0f },
            { StatsDef.HealthRegenRate, 1.0f },
            { StatsDef.DelayBeforeRegen, 1.0f },
            { StatsDef.InvulnerabilityDuration, 0.2f },
            { StatsDef.Damage, 100f },
            { StatsDef.AttackCooldown, 0.5f },
            { StatsDef.MoveSpeed, 3.5f },
            { StatsDef.RunSpeed, 6.0f },
            { StatsDef.StaminaTotal, 100f },
            { StatsDef.StaminaRegenRate, 10f },
            { StatsDef.StaminaWalkRegenRate, 5f },
            { StatsDef.StaminaDepletionRate, 20f },
            { StatsDef.StaminaDepletionThreshold, 30f }
        };

        // Core systems
        public StatCollection Stats { get; private set; }
        public InventoryGridRuntime Inventory { get; private set; }
        public EffectSystem Effects { get; private set; }

        // Optional: standardize orders for everyone
        public static class Orders
        {
            public const int Flat = 100;
            public const int PercentAdd = 200;
            public const int PercentMult = 300;
            public const int FinalAdd = 400;
        }

        public override void Init(CharacterBase characterBase)
        {
            base.Init(characterBase);
            Bootstrap();
        }

        public override void UpdateComponent(float deltaTime)
        {
            base.UpdateComponent(deltaTime);
            Effects.Tick(deltaTime);
        }

        /// <summary>
        /// Initialize core systems and base stats; can be called manually if needed.
        /// </summary>
        private void Bootstrap()
        {
            Stats = new StatCollection(_statsDefinitionsTable);
            Inventory = new InventoryGridRuntime(this);
            Effects = new EffectSystem(Stats);

            foreach (var pair in _statsDefinitionsTable.Rows)
            {
                StatsDef stat = new(pair.Key.ToString());
                if (_baseStatsOverrides.TryGetValue(stat, out var v))
                {
                    Stats.SetBase(stat, v);
                }
            }
        }

    }
}
