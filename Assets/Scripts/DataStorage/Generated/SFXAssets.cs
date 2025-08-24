namespace DataStorage.Generated
{
    [System.Serializable]
    public class SFXAssets: TableID
    {
        public static readonly SFXAssets Steps = new SFXAssets("Steps");
        public static readonly SFXAssets Countdown = new SFXAssets("Countdown");
        public static readonly SFXAssets BallHit = new SFXAssets("BallHit");
        public static readonly SFXAssets GoalScore = new SFXAssets("GoalScore");
        public SFXAssets(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(SFXAssets))]
    public class SFXAssetsPropertyDrawer : TableIDProperyDrawer<SFXAssets> { }
#endif
}
