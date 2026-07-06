using System.Collections.Generic;
using UnityEngine;

public class StatDatabase : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    public PlayerStats PlayerStats => playerStats;

    [SerializeField] private LeviathanStats leviathanStats;

    public LeviathanStats LeviathanStats => leviathanStats;
    public static StatDatabase Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public StatObject[] GetAllStatObjects()
    {
        return Resources.LoadAll<StatObject>("Entities");
    }
}
[System.Serializable]
public class PlayerStats
{
    [Header("Movement")]

    public StatObject PlayerMoveSpeed;
    public StatObject PlayerDecelerationDrag;
    public StatObject PlayerGroundAcceleration;
    public StatObject PlayerGroundedJumpInfo;
    //Air Movement
    public StatObject PlayerAirAcceleration;
    public StatObject PlayerMaxFallSpeed;
    public StatObject PlayerAngleToBeConsideredTurning;
    public StatObject PlayerTurnAngleSpeedLostCurve;

    [Header("Worms")]
    public StatObject PlayerMaxWorms;
    public StatObject PlayerWormsRequiredForRail;
    public StatObject PlayerWormThrowRange;
    public StatObject PlayerWormThrowDuration;
    public StatObject PlayerWormJumpInfo;

    public StatObject PlayerWormGravityFreeTime;
    public StatObject PlayerWormFlySpeed;
    public StatObject PlayerWormGravity;
    public StatObject PlayerWormMaxFallSpeed;
    public StatObject PlayerWormHitsBeforeDeactivation;

    [Header("Rod")]
    public StatObject PlayerMaxRodRange;
    public StatObject PlayerRodSwingMassScale;
    public StatObject PlayerRodSpring;
    public StatObject PlayerRodDamper;
    public StatObject PlayerRodMaxDistanceWithNoSpring;
    public StatObject PlayerRodMinDistanceWithNoSpring;

    [Header("Swinging")]
    public StatObject SwingAcceleration;
    public StatObject SwingJumpInfo;
    public StatObject MinSwingJumpHeight;
    public StatObject SwingSpeedToJumpPowerRatio;

    [Header("Dash")]
    public StatObject PlayerDashGravity;
    public StatObject PlayerDashPower;
    public StatObject PlayerDashLateralAcceleration;
    public StatObject PlayerMaxDashSpeed;
    public StatObject PlayerMinDistanceBeforeDashCancelled;

    [Header("Parry")]
    public StatObject ProperParryDuration;
    public StatObject PartialParryDuration;
    public StatObject ParryStrafeSpeed;
    public StatObject RodLengthAdditionalParrySize;
    public StatObject ParrySpeedIncrease;
    public StatObject PartialParrySpeedPenalty;
    public StatObject ParryBounceControl;
    public StatObject RailParryMinimumSpeed;
    public StatObject RailParryMinimumJump;
    public StatObject PreviousSpeedToRailSpeedRatio;

    [Header("Squashbuckler")]
    public StatObject PlayerChargesToEnterSquashbucklerMode;
    public StatObject PlayerMinimumShadowstepSpeed;
    public StatObject PlayerDurationPerSquashbucklerCharge;
    public StatObject PlayerDragonslashAnarchyRequirement;
    public StatObject PlayerDragonslashSpeedBonusFromRodLength;
    public StatObject PlayerDragonslashSpeed;

    [Header("Anarchy")]
    public StatObject PlayerUniqueAnarchyOptionCountToClearScaling;
    public StatObject PlayerAnarchyScalingGenerationReductionAmount;
    public StatObject PlayerGenerationPerAnarchyOption;
    public StatObject PlayerBaseAnarchyDecayRate;
    public StatObject PlayerMinAnarchyDecayRate;

    [Header("Slash")]
    public StatObject PlayerMinSlashDamage;
    public StatObject PlayerMaxSlashDamage;
    public StatObject PlayerMinDragonslashDamage;
    public StatObject PlayerMaxDragonslashDamage;
    public StatObject PlayerSlashSpeed;
    public StatObject PlayerSlashAnarchyProgressAmount;
    public StatObject PlayerSlashRangeBonusFromRodLength;
    public StatObject PlayerSlashRodExtensionSpeed;
    public StatObject PlayerSpeedToDragonslashDamageCurve;
    public StatObject PlayerSpeedToSlashDamageCurve;

    [Header("Yawn")]
    public StatObject PlayerYawnAirAcceleration;
    public StatObject PlayerMinYawnTime;
    public StatObject PlayerMinJustYawnTime;
    public StatObject PlayerJustYawnWindow;
    public StatObject PlayerJustYawnAnarchyProgress;
    public StatObject PlayerYawnAnarchyProgress;
    public StatObject PlayerRodRetractionSpeedWhileYawning;


    [Header("GetHit")]
    public StatObject ExtraInvulnerabilityFramesAfterHit;
}
//Leviathan Stats

//Movement
[System.Serializable]
public class LeviathanStats
{
    [Header("Movement")]
    public StatObject LeviathanMoveSpeed;
    public StatObject LeviathanMoveAcceleration;
    public StatObject LeviathanMinMoveDuration;
    public StatObject LeviathanMaxMoveDuration;
    public StatObject LeviathanMinIdleDuration;
    public StatObject LeviathanMaxIdleDuration;
    public StatObject LeviathanDecelerationRate;

    [Header("Large Laser")]
    public StatObject LeviathanLargeLaserCooldown;
    public StatObject LeviathanLargeLaserAttackSpeed;

    [Header("Claw Attack")]
    public StatObject LeviathanClawAttackSpeed;
    public StatObject LeviathanClawAttackCooldown;
    public StatObject LeviathanClawAttackLungeDistance;
    public StatObject LeviathanClawAttackRange;


}
