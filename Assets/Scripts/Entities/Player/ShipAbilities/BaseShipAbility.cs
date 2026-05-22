using UnityEngine;

public class BaseShipAbility : BaseEntity
{
    protected AnarchyManager anarchyManager;
    protected PlayerController player;
    public int AnarchyCost { get; protected set; }

    public bool AbilityActive { get; protected set; }

    public virtual void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        this.player = player;
        this.anarchyManager = anarchyManager;
        DeactivateAbility();
    }

    public virtual void ActivateAbility()
    {
        AbilityActive = true;
        anarchyManager.CurrentAnarchy -= AnarchyCost;
    }

    public virtual void DeactivateAbility()
    {
        AbilityActive = false;
    }
    public virtual bool AbilityAvailable()
    {
        return AnarchyCost <= anarchyManager.CurrentAnarchy;
    }
}
