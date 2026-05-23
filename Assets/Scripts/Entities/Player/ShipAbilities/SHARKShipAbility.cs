using UnityEngine;

public class SHARKShipAbility : BaseShipAbility
{
    WormEntity warpTarget;
   [SerializeField] SHARKAbilityData sharkAbilityData;
    public override void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        AnarchyCost = sharkAbilityData.AbilityCost;
        base.InitializeShipAbility(anarchyManager, player);
        player.WormManager.wormRequested += OnWormRequested;
    }

    void OnWormRequested(WormEntity worm)
    {
        warpTarget = worm;
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();
        Vector3 originalPosition = player.RigidBody.position;
        Vector3 reorientDirection = (warpTarget.transform.position - player.transform.position).normalized;
        player.RigidBody.MovePosition(warpTarget.transform.position);
        warpTarget.Rigidbody.MovePosition(originalPosition);
        player.RigidBody.linearVelocity = player.RigidBody.linearVelocity.magnitude * reorientDirection;
        DeactivateAbility();
    }
    public override bool AbilityAvailable()
    {
        if (warpTarget == null)
        {
            return false;
        }
        return base.AbilityAvailable() && warpTarget.wormActive;
    }
}
