namespace DataStorage.Generated
{
    [System.Serializable]
    public class MusicAssets: TableID
    {
        public static readonly MusicAssets Battle1 = new MusicAssets("Battle1");
        public MusicAssets(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(MusicAssets))]
    public class MusicAssetsPropertyDrawer : TableIDProperyDrawer<MusicAssets> { }
#endif
}
