using System.Collections.Generic;

using UnityEngine;

namespace Spawners
{
    public class RandomlyFromSeveralObjectPool<T> : MonoBehaviour, IPool
        where T : PoolObject
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private List<T> _poolObjectPrefabs;
        [SerializeField] private int _initialPoolSize = 10;
        
        private LinkedList<T> _poolObjectList;
        private int _prefabIndex = 0;


        private void Awake()
        {
            _poolObjectList = new LinkedList<T>();
            
            for (int i = 0; i < _initialPoolSize; i++)
            {
                T prefab = CreateObject();
                var newPoolObject = GameObject.Instantiate<T>(prefab, _parent);
                _poolObjectList.AddLast(newPoolObject);

                newPoolObject.Setup(this);
                newPoolObject.gameObject.SetActive(false);
            }
        }

        public T Spawn()
        {
            T resultObject = null;

            if (_poolObjectList.Count > 0)
            {
                int objectsCount = _poolObjectList.Count;
                int objectIndex = Random.Range(0, objectsCount);

                var currentListNode = _poolObjectList.First;

                for (int i = 1; i < objectsCount; i++)
                {
                    currentListNode = currentListNode.Next;

                    if(i == objectIndex)
                    {
                        break;
                    }
                }

                _poolObjectList.Remove(currentListNode);
                resultObject = currentListNode.Value;
            }
            else
            {
                T prefab = CreateObject();
                resultObject = GameObject.Instantiate<T>(prefab, _parent);
                resultObject.Setup(this);
            }

            resultObject.gameObject.SetActive(true);

            return resultObject;
        }

        public void TakeBack(PoolObject poolable)
        {
            T poolableDerrived = (T)poolable;
            _poolObjectList.AddLast(poolableDerrived);
            poolableDerrived.gameObject.SetActive(false);
        }
        
        private T CreateObject()
        {
            T prefab = null;

            if (_prefabIndex >= _poolObjectPrefabs.Count)
            {
                _prefabIndex = 0;
            }

            prefab = _poolObjectPrefabs[_prefabIndex];
            _prefabIndex++;

            return prefab;
        }
    }
}