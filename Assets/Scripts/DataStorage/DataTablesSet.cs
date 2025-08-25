using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace DataStorage
{

    public class DataTablesSet<T> : IDataContainer<T> where T: TableRowBase
    {
        [SerializeField]
        private List<DataTable<T>> _tables = new();

        public override IEnumerable<KeyValuePair<TableID, T>> Rows
        {
            get { return _tables.SelectMany(table => table.Rows); }
        }

        public override IEnumerable<TableID> Identifiers
        {
            get { return _tables.SelectMany(table => table.Identifiers); }
        }

        public override bool Get(TableID id, out T row)
        {
            foreach (var table in _tables)
            {
                if (table.Get(id, out row))
                {
                    return true;
                }
            }
            row = null;
            return false;
        }


    }

}

