using System;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    [SerializeField] IDComponent idComponent;

    public IDComponent IDComponent { get => idComponent; }

    public Action<BaseEntity> entityDestroyed;

    public virtual void Initialize()
    {

    }

    public virtual void Process()
    {

    }

    public virtual void PhysicsProcess()
    {

    }
    /// <summary>
    /// Method is called every fixed update, regardless of the time scale. Use this for physics calculations that need to be consistent even when the game is paused or slowed down.
    /// </summary>
    public virtual void PhysicsProcessConstant()
    {

    }
    private void OnEnable()
    {
      if (EntityManager.Instance != null)  EntityManager.Instance.RegisterEntity(this);
    }

    private void OnDestroy()
    {
        entityDestroyed?.Invoke(this);
    }


}
