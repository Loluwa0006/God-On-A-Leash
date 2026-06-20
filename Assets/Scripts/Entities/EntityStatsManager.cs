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


    public enum InfluenceType
    {
        Uninfluenceable,
        MovementSpeed,
        AttackSpeed,
        AttackDamage,
        Jump,
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
    public enum InfluenceSource
    {
        Inactive, // Used for empty influencer slots
        ChronoTimeSlowOffset,
    }
    public enum InfluenceValueType
    {
        Flat,
        Additive,
        Multiplicative,
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
            MaxRodRange,
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
            ChargesToEnterSquashbucklerMode,
            MinimumShadowstepSpeed,
            DurationPerSquashbucklerCharge,
            DragonslashAnarchyProgressAmount,
            DragonslashSpeedBonusFromRodLength,
            //Anarchy
            UniqueAnarchyOptionCountToClearScaling,
            AnarchyScalingGenerationReductionAmount,
            GenerationPerAnarchyOption,
            BaseAnarchyDecayRate,
            MinAnarchyDecayRate,
            //Slash
            MinSlashDamage,
            MaxSlashDamage,
            MinDragonslashDamage,
            MaxDragonslashDamage,
            SlashSpeed,
            SlashAnarchyProgressAmount,
            SlashRangeBonusFromRodLength,
            SlashRodExtensionSpeed,
            //Yawn
            YawnAirAcceleration,
            MinYawnTime,
            MinJustYawnTime,
            JustYawnWindow,
            YawnAnarchyProgressPerFrame,
            JustYawnAnarchyProgress,
            RodRetractionSpeedWhileYawning,

            //Misc
            ExtraInvulnerabilityFramesAfterHit,

        //Leviathan Stats
        LeviathanMoveSpeed,
        LeviathanMinMoveDuration,
        LeviathanMaxMoveDuration,
    }

    public class StatInfluence
    {
        public float value;
        public int duration;
        public int priority;
        public InfluenceSource source;
        public InfluenceValueType valueType;

        public StatInfluence(float value, int duration, int priority, InfluenceSource source, InfluenceValueType valueType)
        {
            this.value = value;
            this.duration = duration;
            this.priority = priority;
            this.source = source;
            this.valueType = valueType;
        }
    }
    public class StatObject
    {
        public InfluenceType type;
        public float value;
        public StatID ID;
        public StatObject(float value, InfluenceType type, StatID ID)
        {
            this.value = value;
            this.type = type;
            this.ID = ID;
        }
    }


    public Dictionary<InfluenceSource, int> priorityIndex = new()
    {
        { InfluenceSource.ChronoTimeSlowOffset, INFINITE_PRIORITY },
        { InfluenceSource.Inactive, 0 },
    };

    public Dictionary<InfluenceType, float> boostCaps = new()
    {
        { InfluenceType.MovementSpeed, 4.0f },
        { InfluenceType.AttackSpeed, 4.0f },
        { InfluenceType.AttackDamage, 4.0f },
        { InfluenceType.Jump, 4.0f },
        { InfluenceType.WormCount, 4.0f },
        { InfluenceType.WormRange, 4.0f },
        { InfluenceType.RodLength, 4.0f },
        { InfluenceType.RodRetractionSpeed, 4.0f },
        { InfluenceType.ParryDuration, 4.0f },
        { InfluenceType.ParryPower, 4.0f },
        { InfluenceType.SquashbucklerPower, 4.0f },
        { InfluenceType.SquashbucklerLimit, 4.0f },
        { InfluenceType.AnarchyGeneration, 4.0f },
        { InfluenceType.AnarchyScaling, 4.0f },
    };
    protected Dictionary<StatID, StatObject> statRegistry = new();
    protected Dictionary<InfluenceType, StatInfluence[]> influenceRegistry = new();



    public void Start()
    {
        InitializeRegistry();
    }

    protected virtual void InitializeRegistry()
    {
        foreach (var boostType in Enum.GetValues(typeof(InfluenceType)).Cast<InfluenceType>())
        {
            influenceRegistry[boostType] = new StatInfluence[MAX_STAT_INFLUENCERS];
            for (int i = 0; i < MAX_STAT_INFLUENCERS; i++)
            {
                influenceRegistry[boostType][i] = new StatInfluence(0, 0, -1, InfluenceSource.Inactive, InfluenceValueType.Flat);
            }
        }

        foreach (var statID in Enum.GetValues(typeof(StatID)).Cast<StatID>())
        {
            if (!statRegistry.ContainsKey(statID))
            {
                Debug.LogWarning("Could not find stat ID " + statID.ToString());
            }
        }
    }

    public virtual float GetValueFromStat(StatID statID)
    {
        if (!statRegistry.ContainsKey(statID))
        {
            return MISSING_STAT_ID;
        }
        var stat = statRegistry[statID];
        float statValue = stat.value;
        if (stat.type == InfluenceType.Uninfluenceable || !influenceRegistry.ContainsKey(stat.type))
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
            if (influence.source == InfluenceSource.Inactive) continue;
            switch (influence.valueType)
            {
                case InfluenceValueType.Flat:
                    statValue += influence.value;
                    break;
                case InfluenceValueType.Additive:
                    additiveSum += influence.value;
                    break;
                case InfluenceValueType.Multiplicative:
                    multiplierProduct *= 1 + influence.value;
                    break;
            }
            float currentValue = originalValue * additiveSum * multiplierProduct;

        }
        statValue = statValue * additiveSum * multiplierProduct;
        statValue = Mathf.Clamp(statValue, 0, stat.value * cappedBoostValue);
        return statValue;
    }

    public virtual void AddInfluence(InfluenceType type, InfluenceSource source, InfluenceValueType valueType, float value, int duration)
    {
        int priority = priorityIndex[source];
        StatInfluence influence = new(value, duration, priority, source, valueType);
        bool hadEmptySlot = false;
        for (int i = 0; i < MAX_STAT_INFLUENCERS; i++)
        {
            if (influenceRegistry[type][i].source == InfluenceSource.Inactive)
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
    public virtual void RemoveInfluence(InfluenceSource source)
    {
        foreach (var type in influenceRegistry.Keys)
        {
            var sourceArray = influenceRegistry[type];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var influence = sourceArray[i];
                if (influence.source == source)
                {
                    influence.source = InfluenceSource.Inactive;
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
                    influence.source = InfluenceSource.Inactive;
                }
            }
        }
    }
}
