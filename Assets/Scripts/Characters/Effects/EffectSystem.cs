using System.Collections.Generic;
using Characters.Stats;

namespace Characters.Effects
{
    public class TimedEffect
    {
        public readonly object Source; // Scriptable buff asset or runtime instance
        public readonly List<StatModifier> Mods; // Prebuilt modifiers
        public float TimeLeft;

        public TimedEffect(object source, IEnumerable<StatModifier> mods, float duration)
        {
            Source = source;
            Mods = new List<StatModifier>(mods);
            TimeLeft = duration;
        }
    }

    public class EffectSystem
    {
        private readonly StatCollection _stats;
        private readonly List<TimedEffect> _active = new();

        public EffectSystem(StatCollection stats) => _stats = stats;

        public void Add(TimedEffect eff)
        {
            _active.Add(eff);
            foreach (var m in eff.Mods) _stats.AddModifier(m);
        }

        public void Tick(float dt)
        {
            for (var i = _active.Count - 1; i >= 0; --i)
            {
                _active[i].TimeLeft -= dt;
                if (_active[i].TimeLeft <= 0f)
                {
                    _stats.RemoveAllFromSource(_active[i].Source);
                    _active.RemoveAt(i);
                }
            }
        }
    }
}