using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableFactory
{
    private readonly ObjectPool pool;

    public PoolableFactory(ObjectPool pool, IPoolable poolableToCreate, int poolSize)
    {
        this.pool = pool;
        this.pool.CreatePool(poolableToCreate, poolSize);
    }
 
    public IPoolable TryCreateObject()
    {
        IPoolable product = pool.TryGetPooledObject();
        product.OnPoolableObjectEnable();
        return product;
    }
}