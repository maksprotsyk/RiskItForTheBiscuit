using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class ObjectPool
    {
        private const float ShrinkCheckInterval = 30f;

        private readonly GameObject _prefab;
        private readonly Queue<GameObject> _availableObjects = new();
        private readonly int _minSize;

        private int _totalObjectsCount;
        private int _maxActiveObjects;
        private float _timeSinceLastShrinkCheck;

        public ObjectPool(GameObject prefab, int minSize)
        {
            _prefab = prefab;
            _minSize = minSize;
            _totalObjectsCount = minSize;
            _timeSinceLastShrinkCheck = 0f;
            _maxActiveObjects = 0;

            for (int i = 0; i < minSize; i++)
            {
                GameObject obj = GameObject.Instantiate(_prefab);
                obj.SetActive(false);
                _availableObjects.Enqueue(obj);
            }
        }

        public GameObject GetObject()
        {
            if (_availableObjects.Count > 0)
            {
                GameObject obj = _availableObjects.Dequeue();
                obj.SetActive(true);
                UpdateMaxActiveObjects();
                return obj;
            }
            else
            {
                _totalObjectsCount++;
                return Object.Instantiate(_prefab);
            }
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            _availableObjects.Enqueue(obj);
        }

        public void Update(float deltaTime)
        {
            _timeSinceLastShrinkCheck += deltaTime;
            if (_timeSinceLastShrinkCheck >= ShrinkCheckInterval)
            {
                int targetSize = Mathf.Max(_minSize, _maxActiveObjects);
                ChangePoolSize(targetSize);
            }
        }

        private void UpdateMaxActiveObjects()
        {
            int currentActive = _totalObjectsCount - _availableObjects.Count;
            if (currentActive > _maxActiveObjects)
            {
                _maxActiveObjects = currentActive;
                _timeSinceLastShrinkCheck = 0f;
            }
        }

        private void ChangePoolSize(int targetSize)
        {
            if (targetSize < _minSize)
            {
                targetSize = _minSize;
            }

            _maxActiveObjects = 0;
            _timeSinceLastShrinkCheck = 0f;

            if (targetSize > _totalObjectsCount)
            {
                int objectsToAdd = targetSize - _totalObjectsCount;
                for (int i = 0; i < objectsToAdd; i++)
                {
                    GameObject obj = GameObject.Instantiate(_prefab);
                    obj.SetActive(false);
                    _availableObjects.Enqueue(obj);
                }
                _totalObjectsCount += objectsToAdd;
            }
            else
            {
                int objectsToRemove = _totalObjectsCount - targetSize;
                for (int i = 0; i < objectsToRemove; i++)
                {
                    if (_availableObjects.Count > 0)
                    {
                        GameObject obj = _availableObjects.Dequeue();
                        Object.Destroy(obj);
                        _totalObjectsCount--;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot reduce pool size further, all objects are in use.");
                        break;
                    }
                }
            }

            UpdateMaxActiveObjects();
        }
    }
}
