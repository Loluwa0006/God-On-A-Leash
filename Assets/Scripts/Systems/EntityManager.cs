using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }
    Dictionary<IDComponent, BaseEntity> entityRegistry = new ();
    Dictionary<IDComponent.IDType, List<BaseEntity>> entityTypes = new ();
    List<BaseEntity> entityList = new();
    public int PlayerID { get; set; }

    float durationOfTimeScaleChange = 0f;
    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of EntityManager detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        else Instance = this;
        
        var entities = FindObjectsByType<BaseEntity>(sortMode: FindObjectsSortMode.InstanceID);
        foreach (var idType in System.Enum.GetValues(typeof(IDComponent.IDType)).Cast<IDComponent.IDType>())
        {
            entityTypes.Add(idType, new List<BaseEntity>());
        }
        foreach(var entity in entities)
        {
            if (entity.IDComponent == null)
            {
                Debug.LogError($"Entity {entity.name} does not have an IDComponent attached. This entity will not be registered with the EntityManager.");
                continue;
            }
            if (!entityRegistry.ContainsKey(entity.IDComponent))
            {
                entityRegistry.Add(entity.IDComponent, entity);
                entityTypes[entity.IDComponent.ID_Type].Add(entity);
                entity.entityDestroyed += OnEntityDestroyed;
                entityList.Add(entity);
            }
            else
            {
                Debug.LogWarning($"Duplicate IDComponent found on {entity.name}. This entity will not be registered.");
            }
        }
        foreach (var entity in entities) entity.Initialize();
    }
    private void FixedUpdate()
    {
        foreach (var entity in entityList)
        {
            if (entity.enabled) entity.PhysicsProcess();
        }
    }

    private void Update()
    {
        foreach (var entity in entityList)
        {
            if (entity.enabled) entity.Process();
        } 
        if (durationOfTimeScaleChange > 0.001f)
        {
            durationOfTimeScaleChange = Mathf.MoveTowards(durationOfTimeScaleChange, 0, Time.unscaledDeltaTime);
            Debug.Log($"Time scale will reset in " + durationOfTimeScaleChange.ToString("F2") + " seconds.");
            if (durationOfTimeScaleChange <= 0.001f)
            {
                Time.timeScale = 1f;
                Debug.Log("Time scale reset to 1.0");
            }
        }
    }

    void OnEntityDestroyed(BaseEntity entity)
    {
        if (entityRegistry.ContainsValue(entity))
        {
            entityRegistry.Remove(entity.IDComponent);
            entityList.Remove(entity);
        }
    }

    public void RegisterEntity(BaseEntity entity)
    {
        if (entity == null) return;
        if (!entityRegistry.ContainsKey(entity.IDComponent))
        {
            entityRegistry.Add(entity.IDComponent, entity);
            entityTypes[entity.IDComponent.ID_Type].Add(entity);
            entityList.Add(entity);
        }
    }
    public List<BaseEntity> GetEntitiesOfType(IDComponent.IDType type, bool includeInactive = false)
    {
        if (entityTypes.ContainsKey(type))
        {
            if (includeInactive)
            {
                return entityTypes[type];
            }
            else
            {
                return entityTypes[type].Where(entity => entity.gameObject.activeInHierarchy).ToList();
            }
        }
        return null;
    }

    /// <summary>
    /// Sets the update rate for the game. This will affect all entities and systems that rely on Time.deltaTime for their updates.
    /// </summary>
    /// <param name="timeScale"></param>
    /// <param name="requestor"> The entity requesting the time scale change. </param>
    /// <param name="duration"> The duration in frames for which the time scale should be applied. </param>
    public void SetTimeScale(float timeScale, BaseEntity requestor, int duration)
    {
        if (duration <= 0) return;
        Time.timeScale = timeScale;
        durationOfTimeScaleChange = (float) duration / 60; // converts frames to seconds 
        Debug.Log("Duration is " + duration + " in frames.") ;
        Debug.Log($"Time scale set to {timeScale} by {requestor.name} for {durationOfTimeScaleChange.ToString()} seconds.");
    }
}
