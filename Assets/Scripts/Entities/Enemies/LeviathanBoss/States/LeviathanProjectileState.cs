using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeviathanProjectileState : LeviathanBaseState
{
    [Header("Game Objects")]
    [SerializeField] protected AnimationClip animationClip;

    [Header("Stats")]
    [SerializeField] protected StatObject attackSpeedStat;
    [SerializeField] protected StatObject cooldownStat;

    [Header("Fire Info")]

    [SerializeField] protected ProjectileFireInformation projectileFireInfo;

    protected Queue<BaseProjectile> projectilePool;

    [HideInInspector] public bool FireProjectileThisFrame;
    [HideInInspector] public bool ExitStateThisFrame;

    [HideInInspector] public int NumberOfProjectilesToFireThisFrame = 1;

    [HideInInspector] public float DelayBetweenSingleFrameProjectileFiring = 0.0f;
    protected bool firedProjectilePreviously = false;

    protected int cooldownRemaining;

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        projectilePool = new Queue<BaseProjectile>(projectileFireInfo.poolSize);
        base.InitializeState(stateMachine, owner);
        for (int x = 0; x < projectileFireInfo.poolSize; x++)
        {
            var newProjectile = Instantiate(projectileFireInfo.projectilePrefab);
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
            FireProjectiles(DelayBetweenSingleFrameProjectileFiring, NumberOfProjectilesToFireThisFrame);        
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
        newProjectile.EnableProjectile(projectileFireInfo.spawnPoint.position, Leviathan.Target.transform);
        projectilePool.Enqueue(newProjectile);
    }

    public void FireProjectiles(float delayBetweenShots, float numberOfShots)
    {
        StartCoroutine(FireMultipleProjectiles(delayBetweenShots, numberOfShots));
    }

    IEnumerator FireMultipleProjectiles(float delay, float count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(delay);
            FireProjectile();
        }
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
