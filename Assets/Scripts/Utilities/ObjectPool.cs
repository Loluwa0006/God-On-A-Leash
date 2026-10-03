using System.Collections.Generic;
using UnityEngine.UIElements;

public class ObjectPool<T> where T : class
{
    Queue<T> pool = new();

    public void AddToPool(T objType)
    {
        pool.Enqueue(objType);
    }

    public void AddToPool(List<T> objTypes)
    {
        foreach (var objType in objTypes)
        {
            pool.Enqueue(objType);
        }
    }
    public T GetNewObject()
    {
        var nextObject = pool.Dequeue();
        pool.Enqueue(nextObject);
        return nextObject;
    }

    public List<T> GetNewObjects(int count)
    {
        List<T> newObjects = new ();
        for (int i = 0; i < count; i++)
        {
            if (pool.Count > 0)
            {
                newObjects.Add(pool.Dequeue());
            }
            else
            {
                break;
            }
        }
        return newObjects;
    }
    public T PeekNextObject()
    {
        return pool.Peek();
    }

    public bool HasObjectsInPool()
    {
        return pool.Count > 0;
    }

    public bool HasObjectInPool(T objType)
    {
        return pool.Contains(objType);
    }

    public void Clear(bool destroyObjects = false)
    {
        if (destroyObjects)
        {
            foreach (var obj in pool)
            {
                if (obj is UnityEngine.Object unityObj)
                {
                    UnityEngine.Object.Destroy(unityObj);
                }
            }
        }
        pool.Clear();
    }   
    public override bool Equals(object obj)
    {
        if (obj is ObjectPool<T> otherPool)
        {
            return pool.Equals(otherPool.pool);
        }
        return false;
    }

    public static bool operator ==(ObjectPool<T> a, ObjectPool<T> b)
    {
        return a.pool.Equals(b.pool);
    }

    public static bool operator !=(ObjectPool<T> a, ObjectPool<T> b)
    {
        return !a.pool.Equals(b.pool);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
