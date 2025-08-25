using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Stats
{
    public class StatCollection
    {
        public event Action<StatDefinition> OnStatChanged;

        private readonly Dictionary<StatDefinition, float> _baseValues = new();
        private readonly Dictionary<StatDefinition, List<StatModifier>> _mods = new();
        private readonly Dictionary<StatDefinition, float> _cache = new();
        private readonly HashSet<StatDefinition> _dirty = new();

        public void SetBase(StatDefinition stat, float value)
        {
            var v = stat.Clamp(value);
            if (!_baseValues.TryGetValue(stat, out var cur) || !Mathf.Approximately(cur, v))
            {
                _baseValues[stat] = v;
                MarkDirty(stat);
            }
        }

        public float Get(StatDefinition stat)
        {
            if (_dirty.Contains(stat))
                Recalculate(stat);
            if (_cache.TryGetValue(stat, out var v)) return v;
            return stat.DefaultValue;
        }

        public void AddModifier(StatModifier mod)
        {
            if (!_mods.TryGetValue(mod.Stat, out var list))
            {
                list = new List<StatModifier>();
                _mods[mod.Stat] = list;
            }
            list.Add(mod);
            list.Sort((a,b) => a.Order.CompareTo(b.Order));
            MarkDirty(mod.Stat);
        }

        public void RemoveAllFromSource(object source)
        {
            var touched = new HashSet<StatDefinition>();
            foreach (var kv in _mods)
            {
                int removed = kv.Value.RemoveAll(m => ReferenceEquals(m.Source, source));
                if (removed > 0) touched.Add(kv.Key);
            }
            foreach (var stat in touched) MarkDirty(stat);
        }

        private void MarkDirty(StatDefinition stat)
        {
            _dirty.Add(stat);
        }

        private void Recalculate(StatDefinition stat)
        {
            _dirty.Remove(stat);
            float baseVal = _baseValues.TryGetValue(stat, out var b) ? b : stat.DefaultValue;

            float flat = 0f;
            float percentAdd = 0f;
            float percentMult = 1f;
            float finalAdd = 0f;

            if (_mods.TryGetValue(stat, out var list))
            {
                foreach (var m in list)
                {
                    switch (m.Op)
                    {
                        case StatOp.Flat:        flat += m.Value; break;
                        case StatOp.PercentAdd:  percentAdd += m.Value; break;     // 0.10 = +10%
                        case StatOp.PercentMult: percentMult *= (1f + m.Value); break;
                        case StatOp.FinalAdd:    finalAdd += m.Value; break;
                    }
                }
            }

            float result = (baseVal + flat) * (1f + percentAdd);
            result *= percentMult;
            result += finalAdd;
            result = stat.Clamp(result);

            _cache[stat] = result;
            OnStatChanged?.Invoke(stat);
        }
    }
}
