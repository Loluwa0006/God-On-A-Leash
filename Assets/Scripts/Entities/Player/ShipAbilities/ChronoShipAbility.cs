using UnityEngine;

public class ChronoShipAbility : BaseShipAbility
{

    int durationTracker;

    [SerializeField] ChronoAbilityData chronoAbilityData;
    [SerializeField] Animator vfxAnimator;

    public override void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        AnarchyCost = chronoAbilityData.AbilityCost;
        base.InitializeShipAbility(anarchyManager, player);
    }
    public override void ActivateAbility()
    {
        durationTracker = chronoAbilityData.AbilityDuration;
        base.ActivateAbility();
        EntityManager.Instance.ActivateSpecificEntityUpdateMode(player);
        vfxAnimator.SetTrigger("Activate");
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
        EntityManager.Instance.DeactivateSpecificEntityUpdateMode();
        vfxAnimator.SetTrigger("Deactivate");
    }
}
