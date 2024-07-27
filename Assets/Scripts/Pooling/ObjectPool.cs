using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : CustomMonoBehaviour
{
    public int PoolMaxSize => poolSize;
    public Queue<IPoolable> ObjectsInPool => objectPool;

    private Queue<IPoolable> objectPool = new Queue<IPoolable>();
    private IPoolable objectToPool;
    [SerializeField] private int poolSize = 10;

    public void CreatePool(IPoolable objectToPool, int poolMaxSize = 10)
    {
        if (objectPool != null)
        {
            this.objectToPool = objectToPool;
            this.poolSize = poolMaxSize;

        }
        else Debug.LogWarning("Object is not a poolable object.");
    }

    public IPoolable TryGetPooledObject()
    {
        IPoolable pooledObject = null;

        if (objectPool.Count < poolSize)
        {
            pooledObject = NewObject();
        }
        else
        {
            pooledObject = ReuseObject();
        }

        objectPool.Enqueue(pooledObject);
        return pooledObject;
    }

    private IPoolable NewObject()
    {
        GameObject newObject = Instantiate(objectToPool.GameObject, transform.position, transform.rotation);
        IPoolable pooledObject = newObject.GetComponent<IPoolable>();
        pooledObject.GameObject.name = transform.root.name + "_" + objectToPool.GameObject.name + "_" + objectPool.Count;
        pooledObject.GameObject.transform.SetParent(gameObject.transform);

        return pooledObject;
    }
    private IPoolable ReuseObject()
    {
        IPoolable pooledObject = objectPool.Dequeue();
        pooledObject.GameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        pooledObject.GameObject.SetActive(true);

        return pooledObject;
    }

    public int GetNumberOfActivePoolObjects()
    {
        int activeCount = 0;

        foreach (IPoolable poolable in objectPool)
        {
            if (poolable.GameObject.activeSelf)
            {
                activeCount++;
            }
        }
        return activeCount;
    }
    public List<GameObject> GetAllActiveObjects()
    {
        List<GameObject> activeObjects = new List<GameObject>();

        foreach (IPoolable poolable in objectPool)
        {
            if (poolable.GameObject.activeSelf)
            {
                activeObjects.Add(poolable.GameObject);
            }
        }
        return activeObjects;
    }
    public bool CheckIfPoolObjectIsAlreadyActive(IPoolable objectToCheck)
    {
        return objectToCheck.GameObject.activeSelf;
    }
}
