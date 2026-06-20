using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseActor
{
    [SerializeField] List<HealthComponent> healthFragments = new();
    public PlayerController Target { get; private set; }

    public List<HealthComponent> HealthFragments { get => healthFragments; }

    public override void Initialize()
    {
        base.Initialize();
        Target = EntityManager.Instance.GetEntitiesOfType(IDComponent.IDType.Player)[0].GetComponent<PlayerController>();
    }
}

