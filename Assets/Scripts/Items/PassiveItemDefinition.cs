using Characters.Stats;
using DataStorage.Generated;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "RPG/Passive Item", fileName = "NewPassiveItem")]
    public class PassiveItemDefinition : ItemDefinition
    {
        [Header("Consume behavior")]
        [Tooltip("How long the temporary boost lasts when consumed.")]
        [Min(0f)] public float ConsumeDurationSeconds = 8f;

        [Tooltip("Total power during the consume window, relative to the base effect. 2.0 => total 2× (adds +1× on top of base).")]
        [Min(0f)] public float ConsumeTotalMultiplier = 2f;
    }
}