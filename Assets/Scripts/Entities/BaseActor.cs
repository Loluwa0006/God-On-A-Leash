using UnityEngine;
using UnityEngine.Events;

public class BaseActor : BaseEntity
{
    [SerializeField] Rigidbody _rb;
    public Rigidbody RigidBody { get => _rb; }

    [SerializeField] protected EntityStateMachine stateMachine;

    [SerializeField] EntityStatsManager _statsManager;

    public EntityStatsManager StatsManager { get => _statsManager; }

    public UnityEvent<Collision> entityCollision = new();
    public UnityEvent<Collider> entityTriggerEntry = new();

    private void OnCollisionEnter(Collision collision)
    {
        entityCollision.Invoke(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        entityTriggerEntry.Invoke(other);
    }
}
