using UnityEngine;

public class ChronoShipAbility : BaseShipAbility
{

    int durationTracker;

    [SerializeField] ChronoAbilityData chronoAbilityData;
    [SerializeField] Animator vfxAnimator;

    float originalTimescale = 1.0f;

    public override void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        AnarchyCost = chronoAbilityData.AbilityCost;
        base.InitializeShipAbility(anarchyManager, player);
    }
    public override void ActivateAbility()
    {
        durationTracker = Mathf.RoundToInt(chronoAbilityData.AbilityDuration * (1 - chronoAbilityData.TimeSlow));
        Debug.Log("Setting duration tracker to " + durationTracker);
        base.ActivateAbility();
        vfxAnimator.SetTrigger("Activate");
        originalTimescale = Time.timeScale;
        Time.timeScale = 1.0f - chronoAbilityData.TimeSlow;
        float correctionFactor = 1 / (1.0f - chronoAbilityData.TimeSlow);
        player.StatsManager.AddInfluence(
            PlayerStatsManager.InfluenceType.MovementSpeed, 
            PlayerStatsManager.InfluenceSource.ChronoTimeSlowOffset,
            PlayerStatsManager.InfluenceValueType.Multiplicative, 
            correctionFactor,
            PlayerStatsManager.INFINITE_DURATION_INFLUENCE);
        player.RigidBody.linearVelocity *= 1 / Time.timeScale;
    }

    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        durationTracker--;
        if (durationTracker == 0)
        {
            durationTracker = 0;
            DeactivateAbility();
        }
    }

    public override void DeactivateAbility()
    {
        base.DeactivateAbility();
        vfxAnimator.SetTrigger("Deactivate");
        Time.timeScale = originalTimescale;
        player.StatsManager.RemoveInfluence(PlayerStatsManager.InfluenceSource.ChronoTimeSlowOffset);
        player.RigidBody.linearVelocity *= 1 - chronoAbilityData.TimeSlow;
    }

    public override bool AbilityAvailable()
    {
        //can't stack, causes timescale issues
        return base.AbilityAvailable() && !AbilityActive;
    }
}
