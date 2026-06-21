using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LeviathanLargeBeamState : LeviathanBaseState
{
    public const int BEAM_POOL_SIZE = 3;

    [SerializeField] Transform firePoint;
    [SerializeField] BaseProjectile projectilePrefab;

    Queue<BaseProjectile> projectilePool = new(BEAM_POOL_SIZE);
    
    [HideInInspector] public bool FireProjectile;
    [HideInInspector] public bool ExitStateThisFrame;

    bool firedProjectilePreviously = false;

    int cooldownRemaining;

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        for (int x = 0; x < BEAM_POOL_SIZE; x++)
        {
            var newProjectile = Instantiate(projectilePrefab);
            newProjectile.InitializeProjectile(Leviathan);
            newProjectile.name = Leviathan.name + "LargeBeam" + x;
            newProjectile.DisableProjectile();
            projectilePool.Enqueue(newProjectile);
        }
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        firedProjectilePreviously = false;
        FireProjectile = false;
        ExitStateThisFrame = false;
        Leviathan.Animator.SetTrigger(Leviathan.GetAnimationParameterFormatted(LeviathanEntity.AnimationParameter.Trigger_IsAttacking));
        Leviathan.RigidBody.linearVelocity = Vector3.zero;
        Leviathan.Animator.speed = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanLargeLaserAttackSpeed);
    }

    public override void PhysicsProcess()
    {
        if (!firedProjectilePreviously && FireProjectile)
        {
            FireBeam();
        }
        if (ExitStateThisFrame)
        {
            ExitState();
            return;
        }
        firedProjectilePreviously = FireProjectile;
    }

    void FireBeam()
    {
        var newBeam = projectilePool.Dequeue();
        newBeam.EnableProjectile(firePoint.position, Leviathan.Target.transform);
        projectilePool.Enqueue(newBeam);
    }

    public void ExitState()
    {
        cooldownRemaining = (int) Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanLargeLaserCooldown);
        StateMachine.TransitionTo<LeviathanIdleState>();
    }

    public override void InactivePhysicsProcess()
    {
        cooldownRemaining = (int) Mathf.MoveTowards(0, cooldownRemaining, 1);
    }

    public override bool StateAvailable()
    {
        return cooldownRemaining <= 0;
    }
}
