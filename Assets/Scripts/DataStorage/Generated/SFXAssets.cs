namespace DataStorage.Generated
{
    [System.Serializable]
    public class SFXAssets: TableID
    {
        public static readonly SFXAssets OrcHurt1 = new SFXAssets("OrcHurt1");
        public static readonly SFXAssets HumanHurt1 = new SFXAssets("HumanHurt1");
        public static readonly SFXAssets OrcHurt2 = new SFXAssets("OrcHurt2");
        public static readonly SFXAssets OrcHurt3 = new SFXAssets("OrcHurt3");
        public static readonly SFXAssets SwordAttack1 = new SFXAssets("SwordAttack1");
        public SFXAssets(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(SFXAssets))]
    public class SFXAssetsPropertyDrawer : TableIDProperyDrawer<SFXAssets> { }
#endif
}
