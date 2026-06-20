
using UnityEngine;
public class BaseEnemy : BaseEntity
{
    [SerializeField] Rigidbody rigidBody;

    public Rigidbody RigidBody { get => rigidBody; }
    
    public PlayerController Target { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        Target = EntityManager.Instance.GetEntitiesOfType(IDComponent.IDType.Player)[0].GetComponent<PlayerController>();
    }
}
