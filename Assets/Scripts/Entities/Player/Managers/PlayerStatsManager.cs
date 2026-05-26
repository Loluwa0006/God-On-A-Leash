using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerStats;

public class PlayerStatsManager : BaseEntity
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
        Undefined,
        //General Movement
        MoveSpeed,
        DecelerationDrag,
        // Ground Movement
        GroundAcceleration,
        GroundedJumpPower,
        //Air Movement
        AirAcceleration,
        MaxFallSpeed,
        AngleToBeConsideredTurning,
        FallGravity,
        JumpGravity,
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
        DashGravity,
        DashPower,
        DashLateralAcceleration,
        MaxDashSpeed,
        MinDistanceBeforeDashCancelled,
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
        //Yawn
        YawnAirAcceleration,
        MinYawnTime,
        MinJustYawnTime,
        JustYawnWindow,
        YawnAnarchyProgressPerFrame,
        JustYawnAnarchyProgress,
        RodRetractionSpeedWhileYawning ,

    //Misc
    ExtraInvulnerabilityFramesAfterHit,
    }

    [SerializeField] PlayerStats baseStats;

    public PlayerStats BaseStats { get => baseStats; }
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
    Dictionary<StatID, StatObject> statRegistry = new();
    Dictionary<InfluenceType, StatInfluence[]> influenceRegistry = new();


    public void Start()
    {
        InitializeRegistry();
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

    void InitializeRegistry()
    {
        statRegistry[StatID.MoveSpeed] = new StatObject(baseStats.MoveSpeed, InfluenceType.MovementSpeed, StatID.MoveSpeed);
        statRegistry[StatID.GroundAcceleration] = new StatObject(baseStats.GroundAcceleration, InfluenceType.MovementSpeed, StatID.GroundAcceleration);
        statRegistry[StatID.AirAcceleration] = new StatObject(baseStats.AirAcceleration, InfluenceType.MovementSpeed, StatID.AirAcceleration);
        statRegistry[StatID.ParryStrafeSpeed] = new StatObject(baseStats.ParryStrafeSpeed, InfluenceType.MovementSpeed, StatID.ParryStrafeSpeed);
        statRegistry[StatID.YawnAirAcceleration] = new StatObject(baseStats.YawnAirAcceleration, InfluenceType.MovementSpeed, StatID.YawnAirAcceleration);
        statRegistry[StatID.MinimumShadowstepSpeed] = new StatObject(baseStats.MinimumShadowstepSpeed, InfluenceType.MovementSpeed, StatID.MinimumShadowstepSpeed);
        statRegistry[StatID.DashPower] = new StatObject(baseStats.DashPower, InfluenceType.MovementSpeed, StatID.DashPower);
        statRegistry[StatID.DashLateralAcceleration] = new StatObject(baseStats.DashLateralAcceleration, InfluenceType.MovementSpeed, StatID.DashLateralAcceleration);
        statRegistry[StatID.SwingAcceleration] = new StatObject(baseStats.SwingAcceleration, InfluenceType.MovementSpeed, StatID.SwingAcceleration);
        statRegistry[StatID.MaxDashSpeed] = new StatObject(baseStats.MaxDashSpeed, InfluenceType.MovementSpeed, StatID.MaxDashSpeed);
        statRegistry[StatID.RailParryMinimumSpeed] = new StatObject(baseStats.RailParryMinimumSpeed, InfluenceType.MovementSpeed, StatID.RailParryMinimumSpeed);

        statRegistry[StatID.DecelerationDrag] = new StatObject(baseStats.DecelerationDrag, InfluenceType.Uninfluenceable, StatID.DecelerationDrag);
        statRegistry[StatID.AngleToBeConsideredTurning] = new StatObject(baseStats.AngleToBeConsideredTurning, InfluenceType.Uninfluenceable, StatID.AngleToBeConsideredTurning);
        statRegistry[StatID.SwingSpeedToJumpPowerRatio] = new StatObject(baseStats.SwingSpeedToJumpPowerRatio, InfluenceType.Uninfluenceable, StatID.SwingSpeedToJumpPowerRatio);
        statRegistry[StatID.MinDistanceBeforeDashCancelled] = new StatObject(baseStats.MinDistanceBeforeDashCancelled, InfluenceType.Uninfluenceable, StatID.MinDistanceBeforeDashCancelled);
        statRegistry[StatID.ExtraInvulnerabilityFramesAfterHit] = new StatObject(baseStats.ExtraInvulnerabilityFramesAfterHit, InfluenceType.Uninfluenceable, StatID.ExtraInvulnerabilityFramesAfterHit);
        statRegistry[StatID.PreviousSpeedToRailSpeedRatio] = new StatObject(baseStats.PreviousSpeedToRailSpeedRatio, InfluenceType.Uninfluenceable, StatID.PreviousSpeedToRailSpeedRatio);
        statRegistry[StatID.ParryAccelerationInPercent] = new StatObject(baseStats.ParryAccelerationInPercent, InfluenceType.Uninfluenceable, StatID.ParryAccelerationInPercent);
        statRegistry[StatID.MinAnarchyDecayRate] = new StatObject(baseStats.MinAnarchyDecayRate, InfluenceType.Uninfluenceable, StatID.MinAnarchyDecayRate);
        statRegistry[StatID.BaseAnarchyDecayRate] = new StatObject(baseStats.BaseAnarchyDecayRate, InfluenceType.Uninfluenceable, StatID.BaseAnarchyDecayRate);
        statRegistry[StatID.MinYawnTime] = new StatObject(baseStats.MinYawnTime, InfluenceType.Uninfluenceable, StatID.MinYawnTime);
        statRegistry[StatID.MinJustYawnTime] = new StatObject(baseStats.MinJustYawnTime, InfluenceType.Uninfluenceable, StatID.MinJustYawnTime);
        statRegistry[StatID.JustYawnWindow] = new StatObject(baseStats.JustYawnWindow, InfluenceType.Uninfluenceable, StatID.JustYawnWindow);
        statRegistry[StatID.YawnAnarchyProgressPerFrame] = new StatObject(baseStats.YawnAnarchyProgressPerFrame, InfluenceType.Uninfluenceable, StatID.YawnAnarchyProgressPerFrame);
        statRegistry[StatID.PartialParryDuration] = new StatObject(baseStats.PartialParryDuration, InfluenceType.Uninfluenceable, StatID.PartialParryDuration);
        statRegistry[StatID.PartialParrySpeedPenalty] = new StatObject(baseStats.PartialParrySpeedPenalty, InfluenceType.Uninfluenceable, StatID.PartialParrySpeedPenalty);
        statRegistry[StatID.MaxFallSpeed] = new StatObject(baseStats.MaxFallSpeed, InfluenceType.Uninfluenceable, StatID.MaxFallSpeed);
        statRegistry[StatID.WormThrowDuration] = new StatObject(baseStats.WormThrowDuration, InfluenceType.Uninfluenceable, StatID.WormThrowDuration);
        statRegistry[StatID.DashGravity] = new StatObject(baseStats.DashGravity, InfluenceType.Uninfluenceable, StatID.DashGravity);
        statRegistry[StatID.ProperParryDuration] = new StatObject(baseStats.ProperParryDuration, InfluenceType.Uninfluenceable, StatID.ProperParryDuration);
        statRegistry[StatID.ParryBounceControl] = new StatObject(baseStats.ParryBounceControl, InfluenceType.Uninfluenceable, StatID.ParryBounceControl);
        statRegistry[StatID.ChargesToEnterSquashbucklerMode] = new StatObject(baseStats.ChargesToEnterSquashbucklerMode, InfluenceType.Uninfluenceable, StatID.ChargesToEnterSquashbucklerMode);
        statRegistry[StatID.FallGravity] = new StatObject(baseStats.GroundedJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.FallGravity);
        statRegistry[StatID.SwingRiseGravity] = new StatObject(baseStats.SwingJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.SwingRiseGravity);
        statRegistry[StatID.SwingFallGravity] = new StatObject(baseStats.SwingJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.SwingFallGravity);
        statRegistry[StatID.JumpGravity] = new StatObject(baseStats.GroundedJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.JumpGravity);
        statRegistry[StatID.WormJumpGravity] = new StatObject(baseStats.WormThrowJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.WormJumpGravity);
        statRegistry[StatID.WormFallGravity] = new StatObject(baseStats.WormThrowJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.WormFallGravity);
        statRegistry[StatID.AnarchyScalingGenerationReductionAmount] = new StatObject(BaseStats.AnarchyScalingGenerationReductionAmount, InfluenceType.Uninfluenceable, StatID.AnarchyScalingGenerationReductionAmount);
        statRegistry[StatID.WormsRequiredForRail] = new StatObject(baseStats.WormsRequiredForRail, InfluenceType.Uninfluenceable, StatID.WormsRequiredForRail);
        statRegistry[StatID.RodLengthAdditionalParrySize] = new StatObject(baseStats.RodLengthAdditionalParrySize, InfluenceType.Uninfluenceable, StatID.RodLengthAdditionalParrySize);

        statRegistry[StatID.UniqueAnarchyOptionCountToClearScaling] = new StatObject(baseStats.AnarchyScalingGenerationReductionAmount, InfluenceType.AnarchyScaling, StatID.UniqueAnarchyOptionCountToClearScaling);

        statRegistry[StatID.GenerationPerAnarchyOption] = new StatObject(baseStats.GenerationPerAnarchyOption, InfluenceType.AnarchyGeneration, StatID.GenerationPerAnarchyOption);
        statRegistry[StatID.JustYawnAnarchyProgress] = new StatObject(baseStats.JustYawnAnarchyProgress, InfluenceType.AnarchyGeneration, StatID.JustYawnAnarchyProgress);
        statRegistry[StatID.SlashAnarchyProgressAmount] = new StatObject(baseStats.SlashAnarchyProgressAmount, InfluenceType.AnarchyGeneration, StatID.SlashAnarchyProgressAmount);

        statRegistry[StatID.MinSlashDamage] = new StatObject(baseStats.MinSlashDamage, InfluenceType.AttackDamage, StatID.MinSlashDamage);
        statRegistry[StatID.MaxSlashDamage] = new StatObject(baseStats.MaxSlashDamage, InfluenceType.AttackDamage, StatID.MaxSlashDamage);

        statRegistry[StatID.MinDragonslashDamage] = new StatObject(baseStats.MinDragonslashDamage, InfluenceType.SquashbucklerPower, StatID.MinDragonslashDamage);
        statRegistry[StatID.MaxDragonslashDamage] = new StatObject(baseStats.MaxDragonslashDamage, InfluenceType.SquashbucklerPower, StatID.MaxDragonslashDamage);
        statRegistry[StatID.DragonslashSpeedBonusFromRodLength] = new StatObject(baseStats.DragonslashSpeedBonusFromRodLength, InfluenceType.SquashbucklerPower, StatID.DragonslashSpeedBonusFromRodLength);

        statRegistry[StatID.DurationPerSquashbucklerCharge] = new StatObject(baseStats.DurationPerSquashbucklerCharge, InfluenceType.SquashbucklerLimit, StatID.DurationPerSquashbucklerCharge);

        statRegistry[StatID.DragonslashAnarchyProgressAmount] = new StatObject(baseStats.DragonslashAnarchyProgressAmount, InfluenceType.AnarchyGeneration, StatID.DragonslashAnarchyProgressAmount);

        //1.0f represents 100% for animator
        statRegistry[StatID.SlashSpeed] = new StatObject(1.0f, InfluenceType.AttackSpeed, StatID.SlashSpeed);

        statRegistry[StatID.GroundedJumpPower] = new StatObject(baseStats.GroundedJumpInfo.JumpVelocity, InfluenceType.Jump, StatID.GroundedJumpPower);
        statRegistry[StatID.WormJumpPower] = new StatObject(baseStats.WormThrowJumpInfo.JumpVelocity, InfluenceType.Jump, StatID.WormJumpPower);
        statRegistry[StatID.SwingJumpPower] = new StatObject(baseStats.SwingJumpInfo.JumpVelocity, InfluenceType.Jump, StatID.SwingJumpPower);
        statRegistry[StatID.MinSwingJumpHeight] = new StatObject(baseStats.MinSwingJumpHeight, InfluenceType.Jump, StatID.MinSwingJumpHeight);
        statRegistry[StatID.RailParryMinimumJump] = new StatObject(baseStats.RailParryMinimumJump, InfluenceType.Jump, StatID.RailParryMinimumJump);

        statRegistry[StatID.MaxWorms] = new StatObject(baseStats.MaxWorms, InfluenceType.WormCount, StatID.MaxWorms);

        statRegistry[StatID.WormThrowRange] = new StatObject(baseStats.WormThrowRange, InfluenceType.WormRange, StatID.WormJumpPower);

        statRegistry[StatID.MaxRodRange] = new StatObject(baseStats.MaxRodRange, InfluenceType.RodLength, StatID.MaxRodRange);
        statRegistry[StatID.SlashRangeBonusFromRodLength] = new StatObject(baseStats.SlashRangeBonusFromRodLength, InfluenceType.RodLength, StatID.SlashRangeBonusFromRodLength);

        statRegistry[StatID.ParrySpeedIncrease] = new StatObject(baseStats.ParrySpeedIncrease, InfluenceType.ParryPower, StatID.ParrySpeedIncrease);

        statRegistry[StatID.RodRetractionSpeedWhileYawning] = new StatObject(baseStats.RodRetractionSpeedWhileYawning, InfluenceType.RodRetractionSpeed, StatID.RodRetractionSpeedWhileYawning);

        statRegistry[StatID.Undefined] = new StatObject(-1.0f, InfluenceType.Uninfluenceable, StatID.Undefined);
    }
    public float GetValueFromStat(StatID statID)
    {
        if (!statRegistry.ContainsKey(statID))
        {
            return MISSING_STAT_ID;
        }
        var stat = statRegistry[statID];
        float statValue = stat.value;
        if (stat.type == InfluenceType.Uninfluenceable)
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
                    multiplierProduct *=  1 + influence.value;
                    break;
            }
           float currentValue = originalValue * additiveSum * multiplierProduct;

        }
        statValue = statValue * additiveSum * multiplierProduct;
        statValue = Mathf.Clamp(statValue, 0, stat.value * cappedBoostValue);
        return statValue;
    }

    public void AddInfluence(InfluenceType type, InfluenceSource source, InfluenceValueType valueType, float value, int duration)
    {
        int priority = priorityIndex[source];
        StatInfluence influence = new (value, duration, priority, source, valueType);
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
    public void RemoveInfluence(InfluenceSource source)
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

