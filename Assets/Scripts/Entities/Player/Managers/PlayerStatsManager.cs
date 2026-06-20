using System;
using UnityEngine;

public class PlayerStatsManager : EntityStatsManager
{

    [SerializeField] PlayerStats baseStats;

    public PlayerStats BaseStats { get => baseStats; }

    protected override void InitializeRegistry()
    {
        statRegistry[StatID.PlayerMoveSpeed] = new StatObject(baseStats.MoveSpeed, InfluenceType.MovementSpeed, StatID.PlayerMoveSpeed);
        statRegistry[StatID.PlayerGroundAcceleration] = new StatObject(baseStats.GroundAcceleration, InfluenceType.MovementSpeed, StatID.PlayerGroundAcceleration);
        statRegistry[StatID.PlayerAirAcceleration] = new StatObject(baseStats.AirAcceleration, InfluenceType.MovementSpeed, StatID.PlayerAirAcceleration);
        statRegistry[StatID.ParryStrafeSpeed] = new StatObject(baseStats.ParryStrafeSpeed, InfluenceType.MovementSpeed, StatID.ParryStrafeSpeed);
        statRegistry[StatID.YawnAirAcceleration] = new StatObject(baseStats.YawnAirAcceleration, InfluenceType.MovementSpeed, StatID.YawnAirAcceleration);
        statRegistry[StatID.MinimumShadowstepSpeed] = new StatObject(baseStats.MinimumShadowstepSpeed, InfluenceType.MovementSpeed, StatID.MinimumShadowstepSpeed);
        statRegistry[StatID.PlayerDashPower] = new StatObject(baseStats.DashPower, InfluenceType.MovementSpeed, StatID.PlayerDashPower);
        statRegistry[StatID.PlayerDashLateralAcceleration] = new StatObject(baseStats.DashLateralAcceleration, InfluenceType.MovementSpeed, StatID.PlayerDashLateralAcceleration);
        statRegistry[StatID.SwingAcceleration] = new StatObject(baseStats.SwingAcceleration, InfluenceType.MovementSpeed, StatID.SwingAcceleration);
        statRegistry[StatID.PlayerMaxDashSpeed] = new StatObject(baseStats.MaxDashSpeed, InfluenceType.MovementSpeed, StatID.PlayerMaxDashSpeed);
        statRegistry[StatID.RailParryMinimumSpeed] = new StatObject(baseStats.RailParryMinimumSpeed, InfluenceType.MovementSpeed, StatID.RailParryMinimumSpeed);

        statRegistry[StatID.PlayerDecelerationDrag] = new StatObject(baseStats.DecelerationDrag, InfluenceType.Uninfluenceable, StatID.PlayerDecelerationDrag);
        statRegistry[StatID.PlayerAngleToBeConsideredTurning] = new StatObject(baseStats.AngleToBeConsideredTurning, InfluenceType.Uninfluenceable, StatID.PlayerAngleToBeConsideredTurning);
        statRegistry[StatID.SwingSpeedToJumpPowerRatio] = new StatObject(baseStats.SwingSpeedToJumpPowerRatio, InfluenceType.Uninfluenceable, StatID.SwingSpeedToJumpPowerRatio);
        statRegistry[StatID.PlayerMinDistanceBeforeDashCancelled] = new StatObject(baseStats.MinDistanceBeforeDashCancelled, InfluenceType.Uninfluenceable, StatID.PlayerMinDistanceBeforeDashCancelled);
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
        statRegistry[StatID.PlayerMaxFallSpeed] = new StatObject(baseStats.MaxFallSpeed, InfluenceType.Uninfluenceable, StatID.PlayerMaxFallSpeed);
        statRegistry[StatID.WormThrowDuration] = new StatObject(baseStats.WormThrowDuration, InfluenceType.Uninfluenceable, StatID.WormThrowDuration);
        statRegistry[StatID.PlayerDashGravity] = new StatObject(baseStats.DashGravity, InfluenceType.Uninfluenceable, StatID.PlayerDashGravity);
        statRegistry[StatID.ProperParryDuration] = new StatObject(baseStats.ProperParryDuration, InfluenceType.Uninfluenceable, StatID.ProperParryDuration);
        statRegistry[StatID.ParryBounceControl] = new StatObject(baseStats.ParryBounceControl, InfluenceType.Uninfluenceable, StatID.ParryBounceControl);
        statRegistry[StatID.ChargesToEnterSquashbucklerMode] = new StatObject(baseStats.ChargesToEnterSquashbucklerMode, InfluenceType.Uninfluenceable, StatID.ChargesToEnterSquashbucklerMode);
        statRegistry[StatID.PlayerFallGravity] = new StatObject(baseStats.GroundedJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.PlayerFallGravity);
        statRegistry[StatID.SwingRiseGravity] = new StatObject(baseStats.SwingJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.SwingRiseGravity);
        statRegistry[StatID.SwingFallGravity] = new StatObject(baseStats.SwingJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.SwingFallGravity);
        statRegistry[StatID.PlayerJumpGravity] = new StatObject(baseStats.GroundedJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.PlayerJumpGravity);
        statRegistry[StatID.WormJumpGravity] = new StatObject(baseStats.WormThrowJumpInfo.JumpGravity, InfluenceType.Uninfluenceable, StatID.WormJumpGravity);
        statRegistry[StatID.WormFallGravity] = new StatObject(baseStats.WormThrowJumpInfo.FallGravity, InfluenceType.Uninfluenceable, StatID.WormFallGravity);
        statRegistry[StatID.AnarchyScalingGenerationReductionAmount] = new StatObject(BaseStats.AnarchyScalingGenerationReductionAmount, InfluenceType.Uninfluenceable, StatID.AnarchyScalingGenerationReductionAmount);
        statRegistry[StatID.WormsRequiredForRail] = new StatObject(baseStats.WormsRequiredForRail, InfluenceType.Uninfluenceable, StatID.WormsRequiredForRail);
        statRegistry[StatID.RodLengthAdditionalParrySize] = new StatObject(baseStats.RodLengthAdditionalParrySize, InfluenceType.Uninfluenceable, StatID.RodLengthAdditionalParrySize);
        statRegistry[StatID.SlashRodExtensionSpeed] = new StatObject(baseStats.SlashRodExtensionSpeed, InfluenceType.Uninfluenceable, StatID.SlashRodExtensionSpeed);

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

        statRegistry[StatID.PlayerGroundedJumpPower] = new StatObject(baseStats.GroundedJumpInfo.JumpVelocity, InfluenceType.Jump, StatID.PlayerGroundedJumpPower);
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

    

}

