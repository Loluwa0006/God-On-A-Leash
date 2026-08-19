using UnityEngine;
using UnityEngine.Splines;

public class WormRailEntity : BaseEntity
{
    public const int UPDATE_RATE = 5;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] MeshRenderer meshRenderer;

    WormEntity startWorm;
    WormEntity endWorm;

    bool railActive = false;
    int updateTracker = 0;

    BezierKnot ownerKnot = new();
    BezierKnot startWormKnot = new();
    BezierKnot endWormKnot = new();

    public override void Initialize()
    {
        if (splineContainer == null) splineContainer = GetComponent<SplineContainer>();
    }

    public void EnableLine(WormEntity start, WormEntity end, Transform owner)
    {
        transform.position = Vector3.zero;
        meshRenderer.enabled = true;
        railActive = true;
        updateTracker = 0;
        startWorm = start;
        endWorm = end;

        start.wormDisabled += DisableLine;
        end.wormDisabled += DisableLine;
        //means the worm was fired again, so this rail becomes invalid
        start.wormEnabled += DisableLine;
        end.wormEnabled += DisableLine;

        ownerKnot.Position = owner.position;
        splineContainer.Spline.SetKnot(0, ownerKnot);
        startWormKnot.Position = startWorm.transform.position;
        splineContainer.Spline.SetKnot(1, startWormKnot);
        endWormKnot.Position = endWorm.transform.position;
        splineContainer.Spline.SetKnot(2, endWormKnot);
    }

    public void DisableLine(WormEntity disabledWorm)
    {
        meshRenderer.enabled = false;
        //splineCollider.enabled = false;
        railActive = false;
        if (startWorm != null) startWorm.wormDisabled -= DisableLine;
        if (endWorm != null) endWorm.wormDisabled -= DisableLine;
    }

    private void OnDisable()
    {
        if (startWorm != null) startWorm.wormDisabled -= DisableLine;
        if (endWorm != null) endWorm.wormDisabled -= DisableLine;
    }

    public override void PhysicsProcess()
    {
        if (railActive)
        {
            updateTracker = (int)Mathf.MoveTowards(updateTracker, UPDATE_RATE, 1);
            if (updateTracker == UPDATE_RATE)
            {
                if (startWorm.InFlight)
                {
                    startWormKnot.Position = startWorm.transform.position;
                    splineContainer.Spline.SetKnot(1, startWormKnot);
                }
                if (endWorm.InFlight)
                {
                    endWormKnot.Position = endWorm.transform.position;
                    splineContainer.Spline.SetKnot(2, endWormKnot);
                }

               updateTracker = 0;
            }
        }
    }
}
