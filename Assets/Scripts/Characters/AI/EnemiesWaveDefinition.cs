using System.Linq;
using AYellowpaper.SerializedCollections;
using DataStorage.Generated;
using UnityEngine;

namespace Characters.AI
{
    public enum EnemyWaveDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    [CreateAssetMenu(menuName = "Enemies/Wave")]
    public class EnemiesWaveDefinition : ScriptableObject
    {
        [Tooltip("Wave difficulty to calculate rewards / give player time to relax")]
        public EnemyWaveDifficulty Difficulty;

        public SerializedDictionary<Enemies, int> GuaranteedEnemies;
        public SerializedDictionary<Enemies, float> EnemySpawnProbabilities;

        [Min(0)]
        public int MinRandomEnemyCount;

        [Min(0)]
        public int MaxRandomEnemyCount;

        [Min(0.05f)]
        public float RandomEnemiesSpawnInterval;

        [Min(0.05f)]
        public float GuarantedEnemiesSpawnInterval;

        [Min(0f)]
        public float WaveDuration;

        private void OnValidate()
        {
            if (MaxRandomEnemyCount < MinRandomEnemyCount)
            {
                Debug.LogWarning("MaxRandomEnemyCount is less than MinRandomEnemyCount");
            }

            float guarantedEnemiesDuration = GuarantedEnemiesSpawnInterval * GuaranteedEnemies.Sum(p => p.Value);
            float randomEnemiesDuration = RandomEnemiesSpawnInterval * MaxRandomEnemyCount;
            float requiredDuration = Mathf.Max(randomEnemiesDuration, guarantedEnemiesDuration);
            if (WaveDuration < requiredDuration)
            {
                Debug.LogWarning($"WaveDuration was too short, need at list: {requiredDuration:0.00} seconds");
            }
        }
    }

}
