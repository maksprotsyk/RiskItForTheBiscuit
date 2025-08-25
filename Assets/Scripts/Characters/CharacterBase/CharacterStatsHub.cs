using System.Diagnostics;
using Characters.Effects;
using Characters.Inventory;
using Characters.Stats;
using UnityEngine;
using DataStorage;
using DataStorage.Generated;
using AYellowpaper.SerializedCollections;
using Debug = UnityEngine.Debug;

namespace Characters
{
    public class CharacterStatsHub : MonoBehaviour
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

        private void Awake()
        {

            Bootstrap();
        }

        private void Update()
        {
            Effects.Tick(Time.deltaTime);
        }

        /// <summary>
        /// Initialize core systems and base stats; can be called manually if needed.
        /// </summary>
        public void Bootstrap()
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            // keep stats sane in the editor
            if (_baseStatsOverrides.TryGetValue(StatsDef.StaminaTotal, out float staminaTotal) &&
                _baseStatsOverrides.TryGetValue(StatsDef.StaminaDepletionThreshold, out float staminaDepletionThreshold) &&
                staminaDepletionThreshold > staminaTotal)
            {
                _baseStatsOverrides[StatsDef.StaminaDepletionThreshold] = Mathf.Max(0.0f, staminaTotal);
            }
        }
#endif

    }
}
