using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStatsManager : BaseEntity
{
    public const int MAX_STAT_INFLUENCERS = 5;
    public const int INFINITE_DURATION_INFLUENCE = -69420;
    public const int INFINITE_PRIORITY = 69420;
    public const int MISSING_STAT_ID = -6969;
    public const int MISSING_STAT_ARGUMENT = -6969420;


    public Dictionary<StatInfluenceSource, int> priorityIndex = new()
    {
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
    protected Dictionary<StatID, RuntimeStatObject> statRegistry = new();
    protected Dictionary<StatInfluenceType, StatInfluence[]> influenceRegistry = new();

    [SerializeField] protected StatsHolder statsHolder;

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
        var statObjects = statsHolder.StatObjects;
        for (int i = 0; i < statObjects.Count; i++)
        {
            RuntimeStatObject[] statObjectsToAdd = statObjects[i].CreateRuntimeStats();
            for (int x = 0; x < statObjectsToAdd.Length; x++)
            {
                statRegistry[statObjectsToAdd[x].ID] = statObjectsToAdd[x];
            }
        }
        CheckForErrorsInRegistry(statObjects);   
    }

    void CheckForErrorsInRegistry(List<StatObject> statObjects)
    {
        foreach (var statID in Enum.GetValues(typeof(StatID)).Cast<StatID>())
        {
            if (!statRegistry.ContainsKey(statID) && statID != StatID.Undefined)
            {
                Debug.LogWarning("Could not find stat ID " + statID.ToString());
            }
        }

        for (int x = 0; x < statObjects.Count; x++)
        {
            if (statObjects[x].ID == StatID.Undefined && !statObjects[x].RequiresMultipleIDS())
            {
                Debug.LogWarning("ID at index " + x + " is undefined");
                continue;
            }
            for (int y = 0; y < statObjects.Count; y++)
            {
                if (x == y) continue;
                if (statObjects[x] == statObjects[y])
                {
                    Debug.LogWarning("Duplicate stat object found at indexes " + x + " and " + y);
                }
                else
                {
                    if (statObjects[x].ID == statObjects[y].ID)
                    {
                        Debug.LogWarning("Duplicate stat ID " + statObjects[x].ID + " found at indexes " + x + " and " + y);
                    }
                }
            }
        }
    }

    public virtual float GetValueFromStat(StatID statID, float argument = MISSING_STAT_ARGUMENT)
    {
        if (!statRegistry.ContainsKey(statID))
        {
            Debug.LogWarning("Could not find stat ID " + statID.ToString());
            return MISSING_STAT_ID;
        }
        var stat = statRegistry[statID];

        float statValue = MISSING_STAT_ID;
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
                statValue = curve.Evaluate((float)argument);
                break;

        }
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

public enum StatID
{
    //Player Stats
    Undefined,
    //General Movement
    PlayerMoveSpeed,
    PlayerDecelerationDrag,
    // Ground Movement
    PlayerGroundAcceleration,
    PlayerGroundedJumpPower,
    //Air Movement
    PlayerAirAcceleration,
    PlayerMaxFallSpeed,
    PlayerAngleToBeConsideredTurning,
    PlayerFallGravity,
    PlayerJumpGravity,
    //Worms
    MaxWorms,
    WormsRequiredForRail,
    WormThrowRange,
    WormThrowDuration,
    WormJumpPower,
    WormJumpGravity,
    WormFallGravity,
    //Rod
    PlayerMaxRodRange,
    PlayerRodSwingMassScale,
    PlayerRodSpring,
    PlayerRodDamper,
    PlayerRodMaxDistanceWithNoSpring,
    PlayerRodMinDistanceWithNoSpring,
    //Swinging
    SwingAcceleration,
    SwingJumpPower,
    MinSwingJumpHeight,
    SwingSpeedToJumpPowerRatio,
    SwingRiseGravity,
    SwingFallGravity,
    //Dash
    PlayerDashGravity,
    PlayerDashPower,
    PlayerDashLateralAcceleration,
    PlayerMaxDashSpeed,
    PlayerMinDistanceBeforeDashCancelled,
    //Parry
    ProperParryDuration,
    PartialParryDuration,
    ParryAccelerationInPercent,
    ParryStrafeSpeed,
    RodLengthAdditionalParrySize,
    ParrySpeedIncrease,
    PartialParrySpeedPenalty,
    ParryBounceControl,
    RailParryMinimumSpeed,
    RailParryMinimumJump,
    PreviousSpeedToRailSpeedRatio,
    //Squashbuckler
    PlayerChargesToEnterSquashbucklerMode,
    PlayerMinimumShadowstepSpeed,
    PlayerDurationPerSquashbucklerCharge,
    PlayerDragonslashAnarchyRequirement,
    PlayerDragonslashSpeedBonusFromRodLength,
    //Anarchy
    PlayerUniqueAnarchyOptionCountToClearScaling,
    PlayerAnarchyScalingGenerationReductionAmount,
    PlayerGenerationPerAnarchyOption,
    PlayerBaseAnarchyDecayRate,
    PlayerMinAnarchyDecayRate,
    //Slash
    PlayerMinSlashDamage,
    PlayerMaxSlashDamage,
    PlayerMinDragonslashDamage,
    PlayerMaxDragonslashDamage,
    PlayerSlashSpeed,
    PlayerSlashAnarchyProgressAmount,
    PlayerSlashRangeBonusFromRodLength,
    PlayerSlashRodExtensionSpeed,
    PlayerSpeedToDragonslashDamageCurve,
    PlayerSpeedToSlashDamageCurve,
    //Yawn
    PlayerYawnAirAcceleration,
    PlayerMinYawnTime,
    PlayerMinJustYawnTime,
    PlayerJustYawnWindow,
    PlayerYawnAnarchyProgressPerFrame,
    PlayerJustYawnAnarchyProgress,
    PlayerRodRetractionSpeedWhileYawning,

    //Misc
    ExtraInvulnerabilityFramesAfterHit,
    TurnAngleSpeedLostCurve,
    PlayerDragonslashSpeed,


    //Leviathan Stats

    //Movement
    LeviathanMoveSpeed,
    LeviathanMoveAcceleration,
    LeviathanMinMoveDuration,
    LeviathanMaxMoveDuration,
    LeviathanMinIdleDuration,
    LeviathanMaxIdleDuration,
    LeviathanDecelerationRate,
    //Laser
    LeviathanLargeLaserCooldown,
    LeviathanLargeLaserAttackSpeed,
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
    public StatInfluenceType type = StatInfluenceType.Uninfluenceable;
    public object value = 0;
    public StatID ID = StatID.Undefined;
    public StatValueType valueType = StatValueType.Float;

    public RuntimeStatObject(object value, StatInfluenceType type, StatID iD)
    {
        this.value = value;
        this.type = type;
        ID = iD;
    }
}

public enum StatValueType
{
    Float,
    AnimationCurve,
    JumpInfo,
    Percentage,
}
