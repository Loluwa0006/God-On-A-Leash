using System.Collections.Generic;
using UnityEngine;

public class LeviathanProjectileState : LeviathanBaseState
{
    [SerializeField] protected int projectilePoolSize = 3;

    [Header("Game Objects")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected BaseProjectile projectilePrefab;
    [SerializeField] protected AnimationClip animationClip;

    [Header("Stats")]
    [SerializeField] protected StatObject attackSpeedStat;
    [SerializeField] protected StatObject cooldownStat;

    protected Queue<BaseProjectile> projectilePool;

    [HideInInspector] public bool FireProjectileThisFrame;
    [HideInInspector] public bool ExitStateThisFrame;

    protected bool firedProjectilePreviously = false;

    protected int cooldownRemaining;

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        projectilePool = new Queue<BaseProjectile>(projectilePoolSize);
        base.InitializeState(stateMachine, owner);
        for (int x = 0; x < projectilePoolSize; x++)
        {
            var newProjectile = Instantiate(projectilePrefab);
            newProjectile.InitializeProjectile(Leviathan);
            newProjectile.name = Leviathan.name + newProjectile.name + x;
            newProjectile.DisableProjectile();
            projectilePool.Enqueue(newProjectile);
        }
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        firedProjectilePreviously = false;
        FireProjectileThisFrame = false;
        ExitStateThisFrame = false;
        Leviathan.Animator.Play(animationClip.name);
    }

    public override void PhysicsProcess()
    {
        if (!firedProjectilePreviously && FireProjectileThisFrame)
        {
            FireProjectile();
        }
        if (ExitStateThisFrame)
        {
            ExitState();
            return;
        }
        firedProjectilePreviously = FireProjectileThisFrame;
    }
    void FireProjectile()
    {
        var newProjectile = projectilePool.Dequeue();
        newProjectile.EnableProjectile(firePoint.position, Leviathan.Target.transform);
        projectilePool.Enqueue(newProjectile);
    }
    public void ExitState()
    {
        if (cooldownStat != null)
        {
            cooldownRemaining = (int)Leviathan.StatsManager.GetValueFromStat(cooldownStat);
        }
        else
        {
            cooldownRemaining = 0;
        }
        StateMachine.TransitionTo<LeviathanIdleState>();
    }
    public override void InactivePhysicsProcess()
    {
        cooldownRemaining = (int) Mathf.MoveTowards(cooldownRemaining, 0, 1);
    }
    public override bool StateAvailable()
    {
        return cooldownRemaining <= 0;
    }
}
