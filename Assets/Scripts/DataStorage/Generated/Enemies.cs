namespace DataStorage.Generated
{
    [System.Serializable]
    public class Enemies: TableID
    {
        public static readonly Enemies Slime1 = new Enemies("Slime1");
        public static readonly Enemies Slime2 = new Enemies("Slime2");
        public static readonly Enemies Slime3 = new Enemies("Slime3");
        public static readonly Enemies Beholder1 = new Enemies("Beholder1");
        public static readonly Enemies Beholder2 = new Enemies("Beholder2");
        public static readonly Enemies Beholder3 = new Enemies("Beholder3");
        public static readonly Enemies Vampire1 = new Enemies("Vampire1");
        public static readonly Enemies Vampire2 = new Enemies("Vampire2");
        public static readonly Enemies Vampire3 = new Enemies("Vampire3");
        public Enemies(string id): base(id){}
    }
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(Enemies))]
    public class EnemiesPropertyDrawer : TableIDProperyDrawer<Enemies> { }
#endif
}
