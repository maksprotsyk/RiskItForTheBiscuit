using UnityEngine;
using DataStorage;
using Characters.AI;
using System.Collections.Generic;
using System.Collections;
using DataStorage.Generated;
using System.Linq;
using System;
using static UnityEngine.EventSystems.EventTrigger;

namespace Managers
{

    public class EnemyWavesManager : MonoBehaviour
    {
        [SerializeField] private IDataContainer<EnemiesTableRow> _enemiesData;
        [SerializeField] private EnemiesWaveDefinition[] _waves;
        [SerializeField] private float _delayBetweenWaves = 5f;

        [Tooltip("Each cycle contains random waves of the given difficulties, after that the number of enemies increases")]
        [SerializeField] private List<EnemyWaveDifficulty> _wavesCycleDifficulties;
        [SerializeField] private float _enemyNumberIncreaseFactorPerCycle = 0.2f;

        // TODO: think about something better
        private float _enemySpawnDistance = 10.0f;

        private int _currentWave = 0;
        private Transform _playerTransform;
        private float _currentEnemyNumberMultiplier = 1f;

        //private List<CharacterBase> _aliveEnemies = new List<CharacterBase>();

        private void Start()
        {
            _playerTransform = ManagersOwner.GetManager<GameplayManager>().PlayerController.transform;
            StartNextWave();
        }

        private void StartNextWave()
        {
            EnemyWaveDifficulty enemyWaveDifficulty = _wavesCycleDifficulties[_currentWave];
            List<EnemiesWaveDefinition> possibleWaves = _waves.Where(w => w.Difficulty == enemyWaveDifficulty).ToList();
            if (possibleWaves.Count == 0)
            {
                Debug.LogError($"No waves of difficulty {enemyWaveDifficulty}");
                return;
            }

            EnemiesWaveDefinition selectedWave = possibleWaves[UnityEngine.Random.Range(0, possibleWaves.Count)];
            int randomEnemyCount = UnityEngine.Random.Range(selectedWave.MinRandomEnemyCount, selectedWave.MaxRandomEnemyCount + 1);
            randomEnemyCount = Mathf.CeilToInt(randomEnemyCount * _currentEnemyNumberMultiplier);
            float randomEnemySpawnDelay = selectedWave.RandomEnemiesSpawnInterval / _currentEnemyNumberMultiplier;

            Debug.Log($"Starting new wave, number of random enemies: {randomEnemyCount}, number of guaranteed enemies: {selectedWave.GuaranteedEnemies.Values.Sum()}");

            StartCoroutine(GuaranteedEnemiesSpawner(selectedWave.GuaranteedEnemies, selectedWave.GuarantedEnemiesSpawnInterval));
            StartCoroutine(RandomEnemySpawner(selectedWave.EnemySpawnProbabilities, randomEnemySpawnDelay, randomEnemyCount));
            StartCoroutine(StartWaveAfterDelay(selectedWave.WaveDuration + _delayBetweenWaves));

            UpdateWaveIndex();
        }

        private void UpdateWaveIndex()
        {
            _currentWave++;
            if (_currentWave >= _wavesCycleDifficulties.Count)
            {
                _currentWave = 0;
                _currentEnemyNumberMultiplier += _enemyNumberIncreaseFactorPerCycle;
            }
        }

        private IEnumerator StartWaveAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartNextWave();
        }

        private IEnumerator GuaranteedEnemiesSpawner(Dictionary<Enemies, int> guaranteedEnemies, float spawnDelay)
        {
            Dictionary<Enemies, int> enemiesRemainingCount = new(guaranteedEnemies);
            List<Enemies> enemies = enemiesRemainingCount.Keys.ToList();
            while (true)
            {
                enemies.RemoveAll(e => enemiesRemainingCount[e] <= 0);
                if (enemies.Count == 0)
                {
                    yield break;
                }

                foreach (Enemies e in enemies)
                {
                    SpawnEnemy(e);
                    enemiesRemainingCount[e]--;
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        }

        private IEnumerator RandomEnemySpawner(Dictionary<Enemies, float> enemySpawnProbabilities, float spawnDelay, int numberOfEnemies)
        {
            float sumOfProbabilities = enemySpawnProbabilities.Values.Sum();
            List<Enemies> enemies = enemySpawnProbabilities.Keys.ToList();
            while (numberOfEnemies > 0)
            {
                numberOfEnemies--;
                float randomValue = UnityEngine.Random.Range(0, sumOfProbabilities);
                foreach (Enemies enemy in enemies)
                {
                    float probability = enemySpawnProbabilities[enemy];
                    if (randomValue < probability)
                    {
                        SpawnEnemy(enemy);
                        break;
                    }
                    else
                    {
                        randomValue -= probability;
                    }
                }
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        // TODO: use pooling
        private void SpawnEnemy(Enemies enemy)
        {
            if (!_enemiesData.Get(enemy, out EnemiesTableRow row))
            {
                Debug.LogError($"No data for enemy {enemy}");
                return;
            }
            GameObject enemyPrefab = row.Prefab;
            if (enemyPrefab == null)
            {
                Debug.LogError($"No prefab for enemy {enemy}");
                return;
            }

            Vector3 spawnPosition = GetRandomEnemySpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            //Debug.Log($"Spawned enemy {enemy} at {spawnPosition}");
        }

        // TODO: avoid collisions with other objects
        private Vector3 GetRandomEnemySpawnPosition()
        {
            float x = UnityEngine.Random.Range(-1f, 1f);
            float y = UnityEngine.Random.Range(-1f, 1f);
            Vector3 direction = new Vector3(x, y, 0).normalized;
            return _playerTransform.position + direction * _enemySpawnDistance;
        }


    }

}
