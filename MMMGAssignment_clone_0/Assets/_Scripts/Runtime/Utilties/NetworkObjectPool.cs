using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Utilities
{
    public class NetworkObjectPool<T> : Singleton<NetworkObjectPool<T>> where T : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
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
                var instance = PhotonNetwork.Instantiate(objectToPool.name, Vector3.zero, Quaternion.identity);
                instance.transform.SetParent(transform);
                ReturnToPool(instance.GetComponent<T>());
            }
        }

        public void ReturnToPool(T instance)
        {
            _availableObjects.Enqueue(instance);
            _photonView.RPC(nameof(RPC_DisableObject), RpcTarget.All, instance);
        }

        public T GetFromPool()
        {
            if (_availableObjects.Count == 0)
            {
                GrowPool();
            }

            var instance = _availableObjects.Dequeue();
            _photonView.RPC(nameof(RPC_EnableObject), RpcTarget.All, instance);
            return instance;
        }

        [PunRPC]
        private void RPC_EnableObject(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        [PunRPC]
        private void RPC_DisableObject(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        
    }
}