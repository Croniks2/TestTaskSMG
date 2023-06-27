using System.Collections.Generic;

using UnityEngine;

namespace Spawners
{
    public interface IPool
    {
        void TakeBack(PoolObject poolable);
    }
    
    public class ObjectPool<T> : MonoBehaviour, IPool 
        where T : PoolObject
    {
        [SerializeField] private T _poolObjectPrefab;
        [SerializeField] private int _initialPoolSize = 10;
        [SerializeField] private bool _fillPoolFromStart = true;

        private Queue<T> _poolObjectQueue;


        private void Awake()
        {
            _poolObjectQueue = new Queue<T>(_initialPoolSize);

            if(_fillPoolFromStart == true)
            {
                for(int i = 0; i < _initialPoolSize; i++)
                {
                    var newPoolObject = GameObject.Instantiate<T>(_poolObjectPrefab, transform);
                    _poolObjectQueue.Enqueue(newPoolObject);

                    newPoolObject.Setup(this);
                    newPoolObject.gameObject.SetActive(false);
                }
            }
        }

        public T Spawn()
        {
            T resultObject = null;

            if(_poolObjectQueue.Count > 0)
            {
                resultObject = _poolObjectQueue.Dequeue();
            }
            else
            {
                resultObject = GameObject.Instantiate<T>(_poolObjectPrefab, transform);
                resultObject.Setup(this);
            }
            
            resultObject.gameObject.SetActive(true);

            return resultObject;
        }

        public void TakeBack(PoolObject poolable)
        {
            T poolableDerrived = (T)poolable;
            _poolObjectQueue.Enqueue(poolableDerrived);
            poolableDerrived.gameObject.SetActive(false);
        }
    }

    public abstract class PoolObject : MonoBehaviour
    {
        private IPool _pool;

        public void ReturnToPool()
        {
            _pool.TakeBack(this);
        }

        public void Setup(IPool pool)
        {
            _pool = pool;
        }
    }
}