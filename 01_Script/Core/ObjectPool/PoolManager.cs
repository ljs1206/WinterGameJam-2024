using System.Collections.Generic;
using UnityEngine;

namespace LJS.pool{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField] private PoolingListSO _poolList;

        private Dictionary<string, LJS.pool.Pool> _pools;

        protected override void Awake()
        {
            base.Awake();
            _pools = new Dictionary<string, LJS.pool.Pool>();

            foreach (PoolItemSO item in _poolList.list)
            {
                CreatePool(item.prefab, item.count);
            }
        }

        private void CreatePool(GameObject prefab, int count)
        {
            IPoolable poolable = prefab.GetComponent<IPoolable>();
            if (poolable == null)
            {
                //this gameobject does not has poolable interface
                Debug.LogWarning($"Gameobject {prefab.name} has not Ipoolable script");
                return;
            }
            

            LJS.pool.Pool pool = new LJS.pool.Pool(poolable, transform, count);
            _pools.Add(poolable.ItemName, pool);
        }

        public LJS.pool.IPoolable Pop(string itemName)
        {
            if (_pools.ContainsKey(itemName))
            {
                LJS.pool.IPoolable item = _pools[itemName].Pop();

                item.ResetItem();
                return item;
            }
            Debug.LogError($"There is no pool {itemName}");
            return null;
        }

        public void Push(LJS.pool.IPoolable item)
        {
            if (_pools.ContainsKey(item.ItemName))
            {
                _pools[item.ItemName].Push(item);
                return;
            }
            Debug.LogError($"There is no pool {item.ItemName}");
        }
    }
}
