using UnityEngine;

[CreateAssetMenu(fileName = "StatsManager", menuName = "Scriptable Objects/StatsManager")]
public class PlayerStats : ScriptableObject
{
    [System.Serializable]
    public struct JumpInfo
    {
        public float jumpHeight;
        public float jumpTimeToPeak;
        public float jumpTimeToDecent;
        public float JumpGravity { get => 2.0f * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak); }
        public float FallGravity { get => 2.0f * jumpHeight / (jumpTimeToDecent * jumpTimeToDecent); }
        public float JumpVelocity { get => 2.0f * jumpHeight; }

    }
    #region GeneralMovement
    [Header("General Movement")]
    [SerializeField] float moveSpeed = 15;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    #endregion
     #region GroundMovement
    [Header("Ground Movement")]
    [SerializeField] float groundAcceleration = 15 / 7.0f;
    public float GroundAcceleration { get => groundAcceleration; private set => groundAcceleration = value; }

    [SerializeField] float decelerationDrag = 0.9f;
    public float DecelerationDrag { get => decelerationDrag; private set => decelerationDrag = value; }

    [SerializeField] JumpInfo groundedJumpInfo;
    public JumpInfo GroundedJumpInfo { get => groundedJumpInfo; private set => groundedJumpInfo = value; }
    #endregion
    #region AirMovement
    [Header("Air Movement")]
    [SerializeField] float airAcceleration = 15 / 12.0f;
    public float AirAcceleration { get => airAcceleration; private set => airAcceleration = value; }

    [SerializeField] float angleToBeConsideredTurning = 5.0f;
    public float AngleToBeConsideredTurning { get => angleToBeConsideredTurning; private set => angleToBeConsideredTurning = value; }

    [SerializeField] float maxFallSpeed;

    public float MaxFallSpeed { get => Mathf.Abs(maxFallSpeed) * -1; private set => maxFallSpeed = value; }

    [SerializeField] AnimationCurve turnAngleToSpeedLost;
    public AnimationCurve TurnAngleSpeedLostCurve { get => turnAngleToSpeedLost; private set => turnAngleToSpeedLost = value;}
    #endregion
    #region Worms
    [Header("Worms")]
    [SerializeField] int maxWorms = 3;
    public int MaxWorms { get => maxWorms; }

    [SerializeField] int wormsRequiredForRail = 3;

    public int WormsRequiredForRail { get => wormsRequiredForRail; }

    [SerializeField] JumpInfo wormThrowInfo;

    public JumpInfo WormThrowJumpInfo { get => wormThrowInfo; private set => wormThrowInfo = value; }

    
    [SerializeField] float wormThrowRange = 120;

    public float WormThrowRange { get => wormThrowRange; }


    [SerializeField] int wormThrowDuration = 19;

    public int WormThrowDuration { get => wormThrowDuration; }
    #endregion
    #region Rod
    [Header("Rod")]
    [SerializeField] float maxRodRange = 120;
    public float MaxRodRange { get => maxRodRange; private set => maxRodRange = value; }
    #endregion
    #region Swing
    [SerializeField] float swingAcceleration = 8.0f;
    public float SwingAcceleration { get => swingAcceleration; private set => swingAcceleration = value; }

    [SerializeField] JumpInfo swingJumpInfo;
    public JumpInfo SwingJumpInfo { get => swingJumpInfo; private set => swingJumpInfo = value; }

    [SerializeField] float minSwingJumpHeight = 5.0f;
    public float MinSwingJumpHeight { get => minSwingJumpHeight; private set => minSwingJumpHeight = value; }

    [SerializeField, Range(0, 1)] float swingSpeedToJumpPowerRatio = 0.3f;

    public float SwingSpeedToJumpPowerRatio {  get => swingSpeedToJumpPowerRatio;}

    [SerializeField] float rodSpring = 10.0f;

    public float RodSpring { get => rodSpring; private set => rodSpring = value; }

    [SerializeField] float rodDamper = 0.2f;

    public float RodDamper { get => rodDamper; }

    [SerializeField, Range(0,1)] float rodMinDistanceWithNoSpring = 0.2f;

    public float RodMinDistanceWithNoSpring { get => rodMinDistanceWithNoSpring;}

    [SerializeField, Range(0, 1)] float rodMaxDistanceWIthNoSpring = 0.8f;

    public float RodMaxDistanceWithNoSpring { get => rodMaxDistanceWIthNoSpring;}

    [SerializeField] float rodSwingMassScale = 4.5f;

    public float RodSwingMassScale { get => rodSwingMassScale; }
    #endregion
    #region Dash

    [SerializeField] float dashGravity = 8.0f;

    public float DashGravity { get => dashGravity; }

    [SerializeField] float dashPower = 30.0f;

    public float DashPower { get => dashPower; }

    [SerializeField] float dashLateralAcceleration;

    public float DashLateralAcceleration { get => dashLateralAcceleration; }

    [SerializeField] float maxDashSpeed = 100.0f;

    public float MaxDashSpeed { get => maxDashSpeed; }

    [SerializeField] float minDistanceBeforeDashCancelled = 25.0f;

    public float MinDistanceBeforeDashCancelled { get => minDistanceBeforeDashCancelled; }

    #endregion
    #region Parry

    [Header("Parry")]

    [SerializeField] int properParryDuration = 27;
    public int ProperParryDuration { get => properParryDuration; }
    [SerializeField] int partialParryDuration = 13;
    public int PartialParryDuration { get => partialParryDuration; }

    [SerializeField] float parryAccelerationInPercent = 0.1f;

    public float ParryAccelerationInPercent { get => parryAccelerationInPercent; }

    [SerializeField] float parryStrafeSpeed = 40 / 22;

    public float ParryStrafeSpeed { get => parryStrafeSpeed;}

    [SerializeField] float minParryRange = 6.0f;

    public float MinParryRange { get => minParryRange; }

    [SerializeField] float parryRangeIncreaseWithSpeedRatio = 0.25f;

    public float ParryRangeIncreaseWithSpeedRatio {  get => parryRangeIncreaseWithSpeedRatio;}

    [SerializeField, Range(0, 1)] float parrySpeedIncrease = 0.1f;

    public float ParrySpeedIncrease { get => parrySpeedIncrease; }

    [SerializeField, Range(0,1)] float partialParrySpeedPenalty = 0.6f;

    public float PartialParrySpeedPenalty { get => partialParrySpeedPenalty;}

    [SerializeField, Range(0, 1)] float parryBounceControl;
    public float ParryBounceControl { get => parryBounceControl; }

    [Header("Rail Parry")]

    [SerializeField] float railParryMinimumSpeed = 40.0f;
    public float RailParryMinimumSpeed { get => railParryMinimumSpeed; }

    [SerializeField] float railParryMinimumJump = 8.0f;

    public float RailParryMinimumJump { get => railParryMinimumJump; }

    [SerializeField] float previousSpeedToRailSpeedRatio = 2.4f;

    public float PreviousSpeedToRailSpeedRatio { get => previousSpeedToRailSpeedRatio; }
    #endregion
    #region Squashbuckler
    [Header("SquashbucklerPower")]
    [SerializeField] int chargesToEnterSquashbucklerMode = 3;

    public int ChargesToEnterSquashbucklerMode { get => chargesToEnterSquashbucklerMode; }

    [SerializeField] float minimumShadowstepSpeed = 60.0f;

    public float MinimumShadowstepSpeed { get => minimumShadowstepSpeed; }

    [SerializeField] int durationPerSquashbucklerCharge = 15;

    public int DurationPerSquashbucklerCharge { get => durationPerSquashbucklerCharge; }

    [SerializeField] float dragonslashAnarchyProgressAmount = 300.0f;

    public float DragonslashAnarchyProgressAmount { get => dragonslashAnarchyProgressAmount; }
    #endregion
    #region Yawn
    [Header("Yawn")]
    [SerializeField] float yawnAirAcceleration = 40 / 25f;
    public float YawnAirAcceleration { get => yawnAirAcceleration; }

    [SerializeField] int minYawnTime = 24;

    public int MinYawnTime { get => minYawnTime; }

    [SerializeField] int minJustYawnTime = 8;

    public int MinJustYawnTime { get => minJustYawnTime; }

    [SerializeField] int justYawnWindow = 8;

    public int JustYawnWindow { get => justYawnWindow; }

    [SerializeField] float yawnAnarchyProgressPerFrame = 1.75f;

    public float YawnAnarchyProgressPerFrame { get =>  yawnAnarchyProgressPerFrame; }

    [SerializeField] float justYawnAnarchyProgress = 100.0f;

    public float JustYawnAnarchyProgress { get => justYawnAnarchyProgress; }

    [SerializeField] float rodRetractionSpeedWhileYawning = 35.0f;

    public float RodRetractionSpeedWhileYawning { get => rodRetractionSpeedWhileYawning; }

    #endregion
    #region Slash
    [SerializeField] float slashAnarchyProgressAmount = 100.0f;

    public float SlashAnarchyProgressAmount {  get => slashAnarchyProgressAmount; }

    [SerializeField] AnimationCurve speedToSlashDamage;

    public AnimationCurve SpeedToSlashDamageCurve { get => speedToSlashDamage; }

    [SerializeField] AnimationCurve speedToDragonslashDamage;

    public AnimationCurve SpeedToDragonslashDamageCurve { get => speedToDragonslashDamage; }

    [SerializeField] int minSlashDamage = 1;

    public int MinSlashDamage { get => minSlashDamage; }

    [SerializeField] int maxSlashDamage = 8;

    public int MaxSlashDamage { get => maxSlashDamage; }

    [SerializeField] int minDragonslashDamage = 10;

    public int MinDragonslashDamage { get => minDragonslashDamage; }

    [SerializeField] int maxDragonslashDamage = 15;

    public int MaxDragonslashDamage { get => maxDragonslashDamage; }
    #endregion
    #region Anarchy
        [Header("Anarchy")]
    [SerializeField] int uniqueAnarchyOptionCountToClearScaling = 2;
    public int UniqueAnarchyOptionCountToClearScaling { get => uniqueAnarchyOptionCountToClearScaling; }

    [SerializeField, Range(0, 1)] float anarchyScalingGenerationReductionAmount = 0.1f;

    public float AnarchyScalingGenerationReductionAmount { get => anarchyScalingGenerationReductionAmount;}
    [SerializeField] float generationPerAnarchyOption = 30.0f;
    public float GenerationPerAnarchyOption { get => generationPerAnarchyOption;}

    [SerializeField] int baseAnarchyDecayRate = 150;
    public int BaseAnarchyDecayRate { get => baseAnarchyDecayRate;}

    [SerializeField] int minAnarchyDecayRate = 30;

    public int MinAnarchyDecayRate { get => minAnarchyDecayRate; }
    #endregion
    #region Misc
    [Header("Invulnerablity")]
    [SerializeField] int extraInvulnerabilityFramesAfterHit = 45;

    public int ExtraInvulnerabilityFramesAfterHit { get => extraInvulnerabilityFramesAfterHit;}
    #endregion
}