using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
    {
        [SerializeField] private int _amountPerGrowth = 1;
        [SerializeField] private T objectToPool;

        private Queue<T> _availableObjects = new Queue<T>();

        private void Awake()
        {
            GrowPool();
        }

        private void GrowPool()
        {
            for (int i = 0; i < _amountPerGrowth; i++)
            {
                var instance = Instantiate(objectToPool);
                instance.transform.SetParent(transform);
                ReturnToPool(instance);
            }
        }

        public void ReturnToPool(T instance)
        {
            instance.gameObject.SetActive(false);
            _availableObjects.Enqueue(instance);
        }

        public T GetFromPool()
        {
            if (_availableObjects.Count == 0)
            {
                GrowPool();
            }

            var instance = _availableObjects.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }
    }
}