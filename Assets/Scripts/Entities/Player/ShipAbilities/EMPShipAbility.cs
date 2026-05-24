using UnityEngine;

public class EMPShipAbility : BaseShipAbility
{
    [SerializeField] EMPAbilityData empAbilityData;

    int durationTracker = 0;
    public override void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        AnarchyCost = empAbilityData.AbilityCost;
        base.InitializeShipAbility(anarchyManager, player);
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();
        durationTracker = empAbilityData.EMPActiveFrames;
    }
    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        durationTracker--;
        if (durationTracker == 0)
        {
            DeactivateAbility();
        }
        else
        {
            DestroyNearbyProjectiles();
        }
    }

    void DestroyNearbyProjectiles()
    {
        foreach (var projectile in EntityManager.Instance.GetEntitiesOfType(IDComponent.IDType.EnemyProjectile))
        {
            if (Vector3.Distance(projectile.transform.position, player.RigidBody.position) < empAbilityData.EMPRange)
            {
                if (projectile.TryGetComponent(out BaseProjectile projectileComponent))
                {
                    if (projectileComponent.GetModifier<ProjectileEmpoweredModifier>() != null) continue;
                    projectileComponent.DisableProjectile();
                }
            }
        }
    }
}
