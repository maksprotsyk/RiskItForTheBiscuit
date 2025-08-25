using System.Diagnostics;
using Characters.Effects;
using Characters.Inventory;
using Characters.Stats;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Characters
{
    public class CharacterStatsHub : MonoBehaviour
    {
        [Header("Health Stats")]
        public StatDefinition MaxHealth;
        public StatDefinition MaxArmor;
        public StatDefinition HealthRegenRate;
        public StatDefinition DelayBeforeRegen;
        public StatDefinition InvulnerabilityDuration;

        [Header("Attack Stats")]
        public StatDefinition Damage;
        public StatDefinition AttackCooldown;

        [Header("Movement Stats")]
        public StatDefinition MoveSpeed; // walk
        public StatDefinition RunSpeed; // run
        public StatDefinition StaminaTotal;
        public StatDefinition StaminaRegenRate; // idle
        public StatDefinition StaminaWalkRegenRate; // walking
        public StatDefinition StaminaDepletionRate; // running drain
        public StatDefinition StaminaDepletionThreshold;

        [Header("Base Values (authoring)")]
        [Min(0)] public float BaseMaxHealth = 100f;
        [Min(0)] public float BaseMaxArmor;
        public float BaseHealthRegenRate = 1.0f;
        public float BaseDelayBeforeRegen = 1.0f;
        public float BaseInvulnerabilityDuration = 0.2f;

        public float BaseDamage = 10f;
        [Min(0)] public float BaseAttackCooldown = 0.5f;

        [Min(0)] public float BaseMoveSpeed = 3.5f;
        [Min(0)] public float BaseRunSpeed = 6.0f;
        [Min(0)] public float BaseStaminaTotal = 100f;
        public float BaseStaminaRegenRate = 10f; // idle
        public float BaseStaminaWalkRegenRate = 5f; // walk
        [Min(0)] public float BaseStaminaDepletionRate = 20f; // run drain
        [Min(0)] public float BaseStaminaDepletionThreshold = 30f;

        // Core systems
        public StatCollection Stats { get; private set; }
        public InventoryRuntime Inventory { get; private set; }
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
            // Validate early in Editor/Dev builds
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateStatAssets();
#endif

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
            Stats = new StatCollection();
            Inventory = new InventoryRuntime(Stats);
            Effects = new EffectSystem(Stats);

            // Health
            Stats.SetBase(MaxHealth, BaseMaxHealth);
            Stats.SetBase(MaxArmor, BaseMaxArmor);
            Stats.SetBase(HealthRegenRate, BaseHealthRegenRate);
            Stats.SetBase(DelayBeforeRegen, BaseDelayBeforeRegen);
            Stats.SetBase(InvulnerabilityDuration, BaseInvulnerabilityDuration);

            // Attack
            Stats.SetBase(Damage, BaseDamage);
            Stats.SetBase(AttackCooldown, BaseAttackCooldown);

            // Movement + Stamina
            Stats.SetBase(MoveSpeed, BaseMoveSpeed);
            Stats.SetBase(RunSpeed, BaseRunSpeed);
            Stats.SetBase(StaminaTotal, BaseStaminaTotal);
            Stats.SetBase(StaminaRegenRate, BaseStaminaRegenRate);
            Stats.SetBase(StaminaWalkRegenRate, BaseStaminaWalkRegenRate);
            Stats.SetBase(StaminaDepletionRate, BaseStaminaDepletionRate);
            Stats.SetBase(StaminaDepletionThreshold, BaseStaminaDepletionThreshold);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Keep thresholds sane in the editor
            if (BaseStaminaDepletionThreshold > BaseStaminaTotal)
                BaseStaminaDepletionThreshold = Mathf.Max(0f, BaseStaminaTotal);
        }
#endif

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void ValidateStatAssets()
        {
            // Warn once per missing asset; keeps authoring safe
            void Check(StatDefinition s, string name)
            {
                if (s == null) Debug.LogWarning($"[CharacterStatsHub] Missing StatDefinition: {name}", this);
            }

            // Health
            Check(MaxHealth, nameof(MaxHealth));
            Check(MaxArmor, nameof(MaxArmor));
            Check(HealthRegenRate, nameof(HealthRegenRate));
            Check(DelayBeforeRegen, nameof(DelayBeforeRegen));
            Check(InvulnerabilityDuration, nameof(InvulnerabilityDuration));

            // Attack
            Check(Damage, nameof(Damage));
            Check(AttackCooldown, nameof(AttackCooldown));

            // Movement
            Check(MoveSpeed, nameof(MoveSpeed));
            Check(RunSpeed, nameof(RunSpeed));
            Check(StaminaTotal, nameof(StaminaTotal));
            Check(StaminaRegenRate, nameof(StaminaRegenRate));
            Check(StaminaWalkRegenRate, nameof(StaminaWalkRegenRate));
            Check(StaminaDepletionRate, nameof(StaminaDepletionRate));
            Check(StaminaDepletionThreshold, nameof(StaminaDepletionThreshold));
        }
    }
}
