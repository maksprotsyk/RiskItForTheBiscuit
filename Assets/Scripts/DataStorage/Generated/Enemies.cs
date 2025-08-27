namespace DataStorage.Generated
{
    [System.Serializable]
    public class Enemies: TableID
    {
        public static readonly Enemies GreenSlime = new Enemies("GreenSlime");
        public Enemies(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(Enemies))]
    public class EnemiesPropertyDrawer : TableIDProperyDrawer<Enemies> { }
#endif
}
