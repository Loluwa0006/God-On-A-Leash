using UnityEngine;

public class LivingRumShipAbility : BaseShipAbility
{
    Camera viewCamera;
    [SerializeField] BaseProjectile livingRumProjectile;
    [SerializeField] Transform rumTarget;
    [SerializeField] Transform firePoint;
    [SerializeField] LayerMask rumMask;
    [SerializeField] LivingRumAbilityData livingRumAbilityData;

    public override void InitializeShipAbility(AnarchyManager anarchyManager, PlayerController player)
    {
        AnarchyCost = livingRumAbilityData.AbilityCost;
        livingRumProjectile.InitializeProjectile(player);
        base.InitializeShipAbility(anarchyManager, player);
        viewCamera = Camera.main;
        livingRumProjectile.transform.SetParent(null);
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
        if (livingRumProjectile == null)
        {
            livingRumProjectile = GetComponentInChildren<BaseProjectile>();
        }
    }
    override public void ActivateAbility()
    {
        if (livingRumProjectile.Active) livingRumProjectile.DisableProjectile();
        base.ActivateAbility();
        SetRumTarget();
       float velocityToInheritFromOwner = Vector3.Dot((rumTarget.position - player.Collider.bounds.center).normalized, player.RigidBody.linearVelocity.normalized);
        livingRumProjectile.RigidBody.linearVelocity = player.RigidBody.linearVelocity * velocityToInheritFromOwner;
        livingRumProjectile.EnableProjectile(firePoint.position, rumTarget);
    }

    public override void DeactivateAbility()
    {
        base.DeactivateAbility();
        if (livingRumProjectile.ProjectileColliders.Contains(GrappleUtilities.RaycastResult.collider))
        {
            player.RodManager.DisableGrapple();
            //Hack solution for now since worms aren't projectile entities yet
        }
    }

    void SetRumTarget()
    {
        Ray ray = new (player.transform.position, viewCamera.transform.forward);
        rumTarget.position = ray.GetPoint(livingRumAbilityData.RumRange);
    }
    public override void UpdateAbility()
    {
        base.UpdateAbility();
        if (!AbilityActive) return;
        SetRumTarget();
    }
}
