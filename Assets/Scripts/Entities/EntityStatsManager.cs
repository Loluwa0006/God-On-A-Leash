using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EntityStatsManager : BaseEntity
{
    public const int MAX_STAT_INFLUENCERS = 5;
    public const int INFINITE_DURATION_INFLUENCE = -69420;
    public const int INFINITE_PRIORITY = 69420;
    public const int MISSING_STAT_ID = -6969;
    public const int MISSING_STAT_ARGUMENT = -696942069;
    public const int NO_INDEX_IN_STAT = 0;


    public Dictionary<StatInfluenceSource, int> priorityIndex = new()
    {
        //needs infinite priority because the slow should never effect the player .
        { StatInfluenceSource.ChronoTimeSlowOffset, INFINITE_PRIORITY },
        { StatInfluenceSource.Inactive, 0 },
    };

    public Dictionary<StatInfluenceType, float> boostCaps = new()
    {
        { StatInfluenceType.MovementSpeed, 4.0f },
        { StatInfluenceType.AttackSpeed, 4.0f },
        { StatInfluenceType.AttackDamage, 4.0f },
        { StatInfluenceType.JumpPower, 4.0f },
        { StatInfluenceType.WormCount, 4.0f },
        { StatInfluenceType.WormRange, 4.0f },
        { StatInfluenceType.RodLength, 4.0f },
        { StatInfluenceType.RodRetractionSpeed, 4.0f },
        { StatInfluenceType.ParryDuration, 4.0f },
        { StatInfluenceType.ParryPower, 4.0f },
        { StatInfluenceType.SquashbucklerPower, 4.0f },
        { StatInfluenceType.SquashbucklerLimit, 4.0f },
        { StatInfluenceType.AnarchyGeneration, 4.0f },
        { StatInfluenceType.AnarchyScaling, 4.0f },
        { StatInfluenceType.FallSpeed, 4.0f },
    };
    protected Dictionary<RegistryKey, RuntimeStatObject> statRegistry = new();
    protected Dictionary<StatInfluenceType, StatInfluence[]> influenceRegistry = new();

    public UnityEvent<BaseEntity> managerInitialized = new();
    public void Start()
    {
         InitializeRegistry();
    }
    protected virtual void InitializeRegistry()
    {
        foreach (var boostType in Enum.GetValues(typeof(StatInfluenceType)).Cast<StatInfluenceType>())
        {
            influenceRegistry[boostType] = new StatInfluence[MAX_STAT_INFLUENCERS];
            for (int i = 0; i < MAX_STAT_INFLUENCERS; i++)
            {
                influenceRegistry[boostType][i] = new StatInfluence(0, 0, -1, StatInfluenceSource.Inactive, StatInfluenceValueType.Flat);
            }
        }
        var statObjects = StatDatabase.Instance.GetAllStatObjects();
        for (int i = 0; i < statObjects.Length; i++)
        {
            RuntimeStatObject[] statObjectsToAdd = statObjects[i].CreateRuntimeStats();
            for (int x = 0; x < statObjectsToAdd.Length; x++)
            {
                var registryIndex = new RegistryKey(statObjects[i], x);
                statRegistry[registryIndex] = statObjectsToAdd[x];
            }
        }
        CheckForErrorsInRegistry(statObjects);   
        managerInitialized.Invoke(this);
    }

    void CheckForErrorsInRegistry(StatObject[] statObjects) 
    { 
        for (int x = 0; x < statObjects.Length; x++)
        {
            for (int y = 0; y < statObjects.Length; y++)
            {
                if (x == y) continue;
                if (statObjects[x] == statObjects[y])
                {
                    Debug.LogWarning("Duplicate stat object found at indexes " + x + " and " + y);
                }
            }
        }
    }

    float ParseValueFromStat(RegistryKey registryEntry, float argument)
    {
        float statValue = MISSING_STAT_ID;

        var stat = statRegistry[registryEntry];
        switch (stat.valueType)
        {
            case StatValueType.Float:
                statValue = (float)stat.value;
                break;
            case StatValueType.AnimationCurve:
                if (argument == MISSING_STAT_ARGUMENT)
                {
                    Debug.LogWarning("Could not find value to evaluate for animation curve stat");
                    return MISSING_STAT_ID;
                }
                var curve = (AnimationCurve)stat.value;
                statValue = curve.Evaluate(argument);
                break;
            case StatValueType.JumpInfo:
                if (argument == MISSING_STAT_ARGUMENT)
                {
                    Debug.LogWarning("Could not find value to evaluate for jump info stat");
                    return MISSING_STAT_ID;
                }
                switch (argument)
                {
                    case JumpInfo.JUMP_VELOCITY_ID:
                        statValue = ((JumpInfo)stat.value).JumpVelocity;
                        break;
                    case JumpInfo.JUMP_GRAVITY_ID:
                        statValue = ((JumpInfo)stat.value).JumpGravity;
                        break;
                    case JumpInfo.FALL_GRAVITY_ID:
                        statValue = ((JumpInfo)stat.value).FallGravity;
                        break;
                    default:
                        Debug.LogWarning("Could not find value to evaluate for jump info stat");
                        return MISSING_STAT_ID;
                }
                break;
            case StatValueType.Percentage:
                statValue = (float)stat.value;
                break;
            default:
                Debug.LogWarning("Could not find value to evaluate for stat " + registryEntry.StatObject.name);
                return MISSING_STAT_ID;
        }
        return statValue;
    }

    public virtual float GetValueFromStat(StatObject statObject, int index = NO_INDEX_IN_STAT, float argument = MISSING_STAT_ARGUMENT)
    {
        RegistryKey registryEntry = new(statObject, index);
        if (!statRegistry.ContainsKey(registryEntry))
        {
            Debug.LogWarning("Could not find stat ID " + statObject.name.ToString());
            return MISSING_STAT_ID;
        }
        var stat = statRegistry[registryEntry];

        float statValue = ParseValueFromStat(registryEntry, argument);

        if (stat.type == StatInfluenceType.Uninfluenceable || !influenceRegistry.ContainsKey(stat.type))
        {
            return statValue;
        }
        float cappedBoostValue = boostCaps[stat.type];
        float additiveSum = 1.0f;
        StatInfluence[] influences = influenceRegistry[stat.type];
        float multiplierProduct = 1.0f;
        float originalValue = statValue;
        foreach (var influence in influences)
        {
            if (influence.source == StatInfluenceSource.Inactive) continue;
            switch (influence.valueType)
            {
                case StatInfluenceValueType.Flat:
                    statValue += influence.value;
                    break;
                case StatInfluenceValueType.Additive:
                    additiveSum += influence.value;
                    break;
                case StatInfluenceValueType.Multiplicative:
                    multiplierProduct *= 1 + influence.value;
                    break;
            }
            float currentValue = originalValue * additiveSum * multiplierProduct;

        }
        statValue = statValue * additiveSum * multiplierProduct;
        statValue = Mathf.Clamp(statValue, 0, originalValue * cappedBoostValue);
        return statValue;
    }
    public virtual void AddInfluence(StatInfluenceType type, StatInfluenceSource source, StatInfluenceValueType valueType, float value, int duration)
    {
        int priority = priorityIndex[source];
        StatInfluence influence = new(value, duration, priority, source, valueType);
        bool hadEmptySlot = false;
        for (int i = 0; i < MAX_STAT_INFLUENCERS; i++)
        {
            if (influenceRegistry[type][i].source == StatInfluenceSource.Inactive)
            {
                hadEmptySlot = true;
                influenceRegistry[type][i] = influence;
                break;
            }
        }
        if (!hadEmptySlot)
        {
            int lowestPriority = int.MaxValue;
            int index = 0;
            for (int i = 0; i < MAX_STAT_INFLUENCERS; i++)
            {
                if (influenceRegistry[type][i].priority < lowestPriority)
                {
                    index = i;
                    lowestPriority = influenceRegistry[type][i].priority;
                }
            }
            if (priority > lowestPriority)
            {
                influenceRegistry[type][index] = influence;
            }
        }
        Array.Sort(influenceRegistry[type], (a, b) => a.priority.CompareTo(b.priority));
    }
    public virtual void RemoveInfluence(StatInfluenceSource source)
    {
        foreach (var type in influenceRegistry.Keys)
        {
            var sourceArray = influenceRegistry[type];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var influence = sourceArray[i];
                if (influence.source == source)
                {
                    influence.source = StatInfluenceSource.Inactive;
                }
            }
        }
    }
    public override void PhysicsProcess()
    {
        DecrementStatInfluencerDurations();
    }
    void DecrementStatInfluencerDurations()
    {
        foreach (var influenceArray in influenceRegistry.Values)
        {
            for (int i = 0; i < influenceArray.Length; i++)
            {
                var influence = influenceArray[i];
                influence.duration--;
                if (influence.duration <= 0)
                {
                    influence.source = StatInfluenceSource.Inactive;
                }
            }
        }
    }
}
public enum StatInfluenceType
{
    Uninfluenceable,
    MovementSpeed,
    AttackSpeed,
    AttackDamage,
    JumpPower,
    FallSpeed,
    WormCount,
    WormRange,
    RodLength,
    RodRetractionSpeed,
    ParryDuration,
    ParryPower,
    SquashbucklerPower,
    SquashbucklerLimit,
    AnarchyGeneration,
    AnarchyScaling,
}
public enum StatInfluenceSource
{
    Inactive, // Used for empty influencer slots
    ChronoTimeSlowOffset,
}
public enum StatInfluenceValueType
{
    Flat,
    Additive,
    Multiplicative,
}
public class StatInfluence
{
    public float value;
    public int duration;
    public int priority;
    public StatInfluenceSource source;
    public StatInfluenceValueType valueType;
    public StatInfluence(float value, int duration, int priority, StatInfluenceSource source, StatInfluenceValueType valueType)
    {
        this.value = value;
        this.duration = duration;
        this.priority = priority;
        this.source = source;
        this.valueType = valueType;
    }
}
[System.Serializable]
public class RuntimeStatObject
{
    public static string UNDEFINED_ID = "Undefined";
    public StatInfluenceType type = StatInfluenceType.Uninfluenceable;
    public object value = 0;
    public StatValueType valueType = StatValueType.Float;

    public RuntimeStatObject(object value, StatInfluenceType type, StatValueType valueType)
    {
        this.value = value;
        this.type = type;
        this.valueType = valueType;
    }
}

public enum StatValueType
{
    Float,
    AnimationCurve,
    JumpInfo,
    Percentage,
}

public struct RegistryKey
{
    public StatObject StatObject;
    public int Index; 

    public RegistryKey(StatObject statObject, int index)
    {
        this.StatObject = statObject;
        this.Index = index;
    }

    override public bool Equals(object obj)
    {
        if (obj is RegistryKey other)
        {
            return this == other;
        }
        return false;
    }
    public static bool operator ==(RegistryKey a, RegistryKey b)
    {
        return a.StatObject == b.StatObject && a.Index == b.Index;
    }

    public static bool operator !=(RegistryKey a, RegistryKey b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StatObject, Index);
    }
}