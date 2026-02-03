using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Pool<T> : MonoBehaviour where T : Component
{
    public static Pool<T> Instance;
    public Queue<T> pool = new Queue<T>();
    [NonSerialized] private Transform _root;
    public bool IsInitialized() => pool.Count > 0;

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (_root == null)
            _root = transform;
    }
#endif

    public void CheckPoolActivity()
    {
#if UNITY_EDITOR
        if (_root == null)
            _root = transform;
        if(_root.childCount > 0 && pool.Count == 0)
            RebuildQueueFromChildren(_root);
#endif
    }
    public T GetItem(Action<T> callback = null)
    {
        T itemFromPool = pool.Dequeue(); //sıranın BAŞINDAN alma, sıradan çıkartma

        callback?.Invoke(itemFromPool);

        itemFromPool.gameObject.SetActive(true);
        return itemFromPool;
    }

    public void ReleaseItem(T item)
    {
        item.gameObject.SetActive(false);

        if (_root != null)
            item.transform.SetParent(_root, worldPositionStays: false);

        pool.Enqueue(item);//sıraya ekleme (SONDAN))
    }

    public void CreatePool(int amount, Transform root, T prefab)
    {
        _root = root != null ? root : transform;

        if (prefab == null)
        {
            Debug.LogError($"[{name}] Prefab is null.");
            return;
        }
        
        for (int i = 0; i < amount; i++)
        {
            T item = Instantiate(prefab, root);
            item.gameObject.SetActive(false);
            //item.transform.SetParent(root);
            pool.Enqueue(item);
        }
    }

    public void ReleaseItemsToPool(T[] items)
    {
        if (items.Length == 0) return;

        foreach (T item in items)
        {
            ReleaseItem(item);
        }
    }

    public void ClearPool()
    {
        pool.Clear();
    }

    public void RestorePool(T[] items)
    {
        pool.Clear();
        if (items == null) return;

        foreach (T item in items)
        {
            if (item == null) continue;

            if (!item.gameObject.activeSelf)
                pool.Enqueue(item);
        }
    }

#if UNITY_EDITOR
    private void RebuildQueueFromChildren(Transform root)
    {
        if (root == null)
            root = transform;

        _root = root;

        var items = root.GetComponentsInChildren<T>(includeInactive: true);
        RestorePool(items);
    }
#endif
}