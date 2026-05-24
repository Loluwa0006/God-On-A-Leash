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

    public bool UpdateSpecificEntitiesOnly { get; private set; }

    public float TimeScale { get; private set; }

    List<BaseEntity> specificEntitiesToUpdate = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of EntityManager detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
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
    }
    private void FixedUpdate()
    {
        if (UpdateSpecificEntitiesOnly)
        {
            foreach (var entity in specificEntitiesToUpdate)
            {
                if (entity.enabled)
                {
                    entity.PhysicsProcess();
                }
            }
        }
        else
        {
            foreach (var entity in entityList)
            {
                if (entity.enabled)
                {
                    entity.PhysicsProcess();
                }
            }
        }
    }

    private void Update()
    {
        if (UpdateSpecificEntitiesOnly)
        {
            foreach (var entity in specificEntitiesToUpdate)
            {
                if (entity.enabled)
                {
                    entity.Process();
                }
            }
        }
        else
        {
            foreach (var entity in entityList)
            {
                if (entity.enabled)
                {
                    entity.Process();
                }
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

    public void ActivateSpecificEntityUpdateMode(params BaseEntity[] entities)
    {
        specificEntitiesToUpdate.Clear();
        specificEntitiesToUpdate.AddRange(entities);
        UpdateSpecificEntitiesOnly = true;
    }

    public void DeactivateSpecificEntityUpdateMode()
    {
        specificEntitiesToUpdate.Clear();
        UpdateSpecificEntitiesOnly = false;
    }

    public List<BaseEntity> GetEntitiesOfType(IDComponent.IDType type)
    {
        if (entityTypes.ContainsKey(type))
        {
            return entityTypes[type];
        }
        return null;
    }
}
