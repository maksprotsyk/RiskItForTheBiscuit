namespace DataStorage.Generated
{
    [System.Serializable]
    public class StatsDef: TableID
    {
        public static readonly StatsDef MaxHealth = new StatsDef("MaxHealth");
        public static readonly StatsDef MaxArmor = new StatsDef("MaxArmor");
        public static readonly StatsDef HealthRegenRate = new StatsDef("HealthRegenRate");
        public static readonly StatsDef DelayBeforeRegen = new StatsDef("DelayBeforeRegen");
        public static readonly StatsDef InvulnerabilityDuration = new StatsDef("InvulnerabilityDuration");
        public static readonly StatsDef Damage = new StatsDef("Damage");
        public static readonly StatsDef AttackCooldown = new StatsDef("AttackCooldown");
        public static readonly StatsDef MoveSpeed = new StatsDef("MoveSpeed");
        public static readonly StatsDef RunSpeed = new StatsDef("RunSpeed");
        public static readonly StatsDef StaminaTotal = new StatsDef("StaminaTotal");
        public static readonly StatsDef StaminaRegenRate = new StatsDef("StaminaRegenRate");
        public static readonly StatsDef StaminaWalkRegenRate = new StatsDef("StaminaWalkRegenRate");
        public static readonly StatsDef StaminaDepletionRate = new StatsDef("StaminaDepletionRate");
        public static readonly StatsDef StaminaDepletionThreshold = new StatsDef("StaminaDepletionThreshold");
        public StatsDef(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(StatsDef))]
    public class StatsDefPropertyDrawer : TableIDProperyDrawer<StatsDef> { }
#endif
}
