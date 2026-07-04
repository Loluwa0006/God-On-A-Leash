using System;
using UnityEngine;
using System.Collections.Generic;

public class BaseProjectile : BaseEntity
{
    [SerializeField] GameObject modifierHolder;
    [SerializeField] protected Rigidbody rigidBody;
    [SerializeField] protected List<Collider> projectileColliders;
    [SerializeField] protected GameObject meshObjects;

    public Rigidbody RigidBody { get => rigidBody; }
    public List<Collider> ProjectileColliders { get => projectileColliders; }
    BaseProjectileModifier[] projectileModifiers;

    public event Action ProjectileFired;
    public event Action ProjectileDestroyed;
    public Action<HealthComponent> ProjectileLanded;

    public Transform Target { get; private set; }

    public bool Active { set; get; } = false;

    public BaseEntity ProjectileOwner { set; get; }
    public void InitializeProjectile(BaseEntity entity)
    {
        ProjectileOwner = entity;
        InitializeModifiers();
        OrderModifiersByPriority();
        EntityManager.Instance.RegisterEntity(this);
        ProjectileLanded += OnProjectileLanded;
    }

    void InitializeModifiers()
    {
        projectileModifiers = modifierHolder.GetComponents<BaseProjectileModifier>();
        for (int i = 0; i < projectileModifiers.Length; i++)
        {
            projectileModifiers[i].InitializeModifier(this);
        }
    }

    void OrderModifiersByPriority()
    {
        Array.Sort(projectileModifiers, (a, b) => a.Priority.CompareTo(b.Priority));
    }

    public override void PhysicsProcess()
    {
        if (Active)
        {
            for (int i = 0; i < projectileModifiers.Length; i++)
            {
                projectileModifiers[i].UpdateModifier();
            }
        }
        else
        {
            for (int i = 0; i < projectileModifiers.Length; i++)
            {
                projectileModifiers[i].InactiveUpdateModifier();
            }
        }
    }

    public void EnableProjectile(Vector3 start, Transform target)
    {
        rigidBody.MovePosition(start);
        Target = target;
        meshObjects.SetActive(true);
        ProjectileFired?.Invoke();
        for (int i = 0; i < projectileColliders.Count; i++)
        {
            projectileColliders[i].enabled = true;
        }
        Active = true;
        for (int i = 0; i < projectileModifiers.Length; i++)
        {
            projectileModifiers[i].OnProjectileFired();
        }
    }
    public void DisableProjectile()
    {
        meshObjects.SetActive(false);
        ProjectileDestroyed?.Invoke();
        for (int i = 0; i < projectileColliders.Count; i++)
        {
            projectileColliders[i].enabled = false;
        }
        Active = false;
        for (int i = 0; i < projectileModifiers.Length; i++)
        {
            projectileModifiers[i].OnProjectileDisabled();
        }
    }

    public void OnProjectileLanded(HealthComponent victim)
    {
        for (int i = 0; i < projectileModifiers.Length; i++)
        {
            projectileModifiers[i].OnProjectileLanded(victim);
        }
    }

    public T GetModifier<T>() where T: BaseProjectileModifier
    {
        for (int i = 0; i < projectileModifiers.Length; i++)
        {
            if (projectileModifiers[i].GetType() == typeof(T))  
            {
                return projectileModifiers[i] as T;
            }
        }
    return null;
    }

}
[System.Serializable]
public struct ProjectileFireInformation
{
    public Transform spawnPoint;
    public int delayBeforeFiring;
    public BaseProjectile projectilePrefab;
    public int fireCooldown;
    public int poolSize;
}

