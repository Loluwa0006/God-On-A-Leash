using System.Collections.Generic;
using UnityEngine;
public class TestEnemy : BaseActor
{
    public enum FireState
    {
        Preparing,
        Reloading,
    }
    public enum ProjectileInfoID
    {
        SniperShot,
        RapidFire
    }

    [System.Serializable]
    public struct TestEnemyProjectileInformation
    {
        public ProjectileInfoID ID;
        public ProjectileFireInformation fireInformation;
        public Queue<BaseProjectile> projectilePool;
    }

    [SerializeField] PlayerController player;
    [SerializeField] List<TestEnemyProjectileInformation> testEnemyProjectileInfos = new();
    [SerializeField] float distanceToUseSniperFireAt = 100.0f;

    Dictionary<ProjectileInfoID, TestEnemyProjectileInformation> projectileRegistry = new();

    ProjectileInfoID queuedProjectileFire;
    FireState fireState = FireState.Reloading;
    int delayRemaining = 0;
    int cooldownRemaining = 0;
    
    private void Start()
    {
        for (int i = 0; i < testEnemyProjectileInfos.Count; i++)
        {
            Queue<BaseProjectile> projectilePool = new();
            var nextInfo = testEnemyProjectileInfos[i];
            for (int x = 0; x < nextInfo.fireInformation.poolSize; x++)
            {
                var newProjectile = Instantiate(nextInfo.fireInformation.projectilePrefab);
                newProjectile.InitializeProjectile(this);
                newProjectile.name = name + nextInfo.ID.ToString() + i;
                newProjectile.DisableProjectile();
                projectilePool.Enqueue(newProjectile);
            }
            nextInfo.projectilePool = projectilePool;
            testEnemyProjectileInfos[i] = nextInfo;
            projectileRegistry[nextInfo.ID] = nextInfo;
        }
    }
    public void PrepareToFire()
    {
        SetFireType();
        fireState = FireState.Preparing;
        delayRemaining = projectileRegistry[queuedProjectileFire].fireInformation.delayBeforeFiring;
    }

    void FireProjectile()
    {
        fireState = FireState.Reloading;
        var queueInfo = projectileRegistry[queuedProjectileFire];
        var nextProjectile = queueInfo.projectilePool.Dequeue();
        nextProjectile.RigidBody.MovePosition(queueInfo.fireInformation.spawnPoint.position);
        nextProjectile.EnableProjectile(player.transform);
        projectileRegistry[queuedProjectileFire].projectilePool.Enqueue(nextProjectile);
        cooldownRemaining = projectileRegistry[queuedProjectileFire].fireInformation.fireCooldown;
    }

    void SetFireType()
    {
        var distance = Vector3.Distance(player.RigidBody.position, RigidBody.position);
        if (distance < distanceToUseSniperFireAt)
        {
            queuedProjectileFire = ProjectileInfoID.RapidFire;
        }
        else
        {
            queuedProjectileFire = ProjectileInfoID.SniperShot;
        }
    }

    public override void PhysicsProcess()
    {
        switch (fireState)
        {
            case FireState.Reloading:
                cooldownRemaining = (int)Mathf.MoveTowards(cooldownRemaining, 0, 1);
                if (cooldownRemaining < 0.001f)
                {
                    PrepareToFire();
                }
                break;
            case FireState.Preparing:
                delayRemaining = (int)Mathf.MoveTowards(delayRemaining, 0, 1);
                if (delayRemaining < 0.001f)
                {
                    FireProjectile();
                }
                break;
        }
    }



}
