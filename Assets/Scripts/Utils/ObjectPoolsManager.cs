using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class ObjectPoolsManager
    {
        private Dictionary<GameObject, ObjectPool> _pools = new();
        private Dictionary<GameObject, GameObject> _objectToPrefabMap = new();

        public void AddPool(GameObject prefab, int minSize)
        {
            if (_pools.ContainsKey(prefab))
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} already exists.");
                return;
            }
            ObjectPool newPool = new(prefab, minSize);
            _pools[prefab] = newPool;
        }

        public GameObject GetObject(GameObject prefab)
        {
            if (!_pools.ContainsKey(prefab))
            {
                Debug.LogError($"No pool found for prefab {prefab.name}. Please add a pool first.");
                return null;
            }
            GameObject obj = _pools[prefab].GetObject();
            _objectToPrefabMap[obj] = prefab;
            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            if (!_objectToPrefabMap.ContainsKey(obj))
            {
                Debug.LogError("This object was not obtained from any pool managed by ObjectPoolsManager.");
                return;
            }

            GameObject prefab = _objectToPrefabMap[obj];
            if (_pools.ContainsKey(prefab))
            {
                _pools[prefab].ReturnObject(obj);
                _objectToPrefabMap.Remove(obj);
            }
            else
            {
                Debug.LogError($"No pool found for prefab {prefab.name}. Cannot return object to pool.");
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var pool in _pools.Values)
            {
                pool.Update(deltaTime);
            }
        }
    }
}
