using System;
using Characters.Stats;

namespace DataStorage
{
    [Serializable]
    public class StatsTableRow : TableRowBase
    {
        public StatDefinition definition;
    }
}
