using UnityEngine;
using UnityEngine.Events;

public class BaseActor : BaseEntity
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] protected EntityStateMachine stateMachine;
    [SerializeField] EntityStatsManager _statsManager;

    [SerializeField] Animator _animator;

    public Animator Animator { get => _animator; }
    public EntityStatsManager StatsManager { get => _statsManager; }
    public Rigidbody RigidBody { get => _rb; }

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
