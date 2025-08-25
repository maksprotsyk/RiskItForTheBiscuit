using DataStorage;
using UnityEngine;

namespace Characters.Stats
{
    [CreateAssetMenu(menuName = "RPG/Stat Definition")]
    public class StatDefinition : ScriptableObject
    {
        [SerializeField] private float _defaultValue = 0f;
        [SerializeField] private float _min = float.NegativeInfinity;
        [SerializeField] private float _max = float.PositiveInfinity;
        public float DefaultValue => _defaultValue;
        public float Clamp(float v) => Mathf.Clamp(v, _min, _max);
    }
}
