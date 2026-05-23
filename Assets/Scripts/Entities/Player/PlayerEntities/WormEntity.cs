using UnityEngine;

public class WormEntity : BaseEntity
{
    [SerializeField] int gravityFreeTime = 7 * 60;
    [SerializeField] float distanceWhereTargetConsideredReached = 8.0f;
    [SerializeField] float flySpeed = 60.0f;
    [SerializeField] float gravity = 8.0f;
    [SerializeField] float maxFallSpeed = 30.0f;

    [SerializeField] MeshRenderer model;
    [SerializeField] Collider swingbox;
    [SerializeField] Rigidbody rigidBody;

    Vector3 target;
    int gravityTracker = 0;
    public bool wormActive { get; private set; }
    public Rigidbody Rigidbody { get => rigidBody; }
    bool reachedTarget = false;
   public void Fire(Vector3 target, Vector3 startingLocation, Vector3 ownerVelocity)
    {
        rigidBody.isKinematic = false;
        this.target = target;
        gravityTracker = gravityFreeTime;
        reachedTarget = false;
        transform.position = startingLocation;
        Vector3 direction = (target - startingLocation).normalized;

        float velocityToInheritFromOwner = Vector3.Dot(direction, ownerVelocity.normalized);

        rigidBody.linearVelocity = direction * flySpeed + ownerVelocity * velocityToInheritFromOwner;

        wormActive = true;
        model.enabled = true;
        swingbox.enabled = true;
    }

    public void Deactivate()
    {
        swingbox.enabled = false;
        model.enabled = false;
        rigidBody.isKinematic = true;
        wormActive = false;
    }

    public override void PhysicsProcess()
    {
        if (!wormActive) return;
        GravityLogic();
        TargetLogic();
    }

    void GravityLogic()
    {
        if (gravityTracker > 0)
        {
            gravityTracker--;
        }
        if (gravityTracker == 0)
        {

            var gravityForce = gravity;
            float differenceBetweenCurrentSpeedAndMaxFallSpeed = Mathf.Abs(rigidBody.linearVelocity.y - maxFallSpeed);
            if (differenceBetweenCurrentSpeedAndMaxFallSpeed < gravityForce)
            {
                gravityForce = differenceBetweenCurrentSpeedAndMaxFallSpeed;
            }
            rigidBody.AddForce(Vector3.down * gravityForce, ForceMode.VelocityChange);
        }
    }

    void TargetLogic()
    {
        if (reachedTarget) return;
        //if (Vector3.Distance(transform.position, target) <= distanceWhereTargetConsideredReached)
        //{
        //    rigidBody.linearVelocity = 
        //    reachedTarget = true;
        //}
    }


}
