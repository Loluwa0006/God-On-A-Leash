using TMPro;
using UnityEngine;

public class RodManager : MonoBehaviour
{
    [SerializeField] LineRenderer rodLine;
    [SerializeField] LayerMask grappleMask;
    [SerializeField] PlayerController player;

    [SerializeField] Transform grapplePoint;


    [SerializeField] TMP_Text rodLengthDisplay;

    bool grappleActive = true;

    SpringJoint grappleJoint;

    public struct GrappleData
    {
        public Collider collider;
        public Vector3 offset;

        public Vector3 GrapplePosition { get => collider.bounds.center + offset;  }
    }

    GrappleData grappleInfo;

    public GrappleData GrappleInfo { set => grappleInfo = value; get => grappleInfo; }
    public LayerMask GrappleMask { get => grappleMask; }

    float rodLength;

    public float RodLength 
    {
        set
        {
            if (rodLengthDisplay != null)
            {
                rodLengthDisplay.text = Mathf.RoundToInt(value).ToString();
            }
            rodLength = Mathf.Clamp(value, 0.0f, player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxRodRange));
        }

        get => rodLength;
    } 

    private void Start()
    {
        DisableGrapple();
    }

    public void StartSwing()
    {
        if (GrappleUtilities.RaycastResult.collider != null)
        {
            grappleInfo.collider = GrappleUtilities.RaycastResult.collider;
            grappleInfo.offset = GrappleUtilities.RaycastResult.point - grappleInfo.collider.bounds.center;

            grappleJoint = player.gameObject.AddComponent<SpringJoint>();
            grappleJoint.autoConfigureConnectedAnchor = false;
            grappleJoint.connectedAnchor = grappleInfo.GrapplePosition;

            grappleJoint.massScale = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSwingMassScale);
            grappleJoint.spring = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSpring);
            grappleJoint.damper = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodDamper);

            var distance = Vector3.Distance(grappleInfo.GrapplePosition, player.Collider.bounds.center);

            grappleJoint.maxDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMaxDistanceWithNoSpring) * distance;
            grappleJoint.minDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMinDistanceWithNoSpring) * distance;

            grappleActive = true;
            rodLine.enabled = true;

            RodLength = Vector3.Distance(player.RigidBody.position, GrappleUtilities.RaycastResult.point);
        }
    }

    public void StartDash()
    {
        if (GrappleUtilities.RaycastResult.collider != null)
        {
            grappleActive = true;
            rodLine.enabled = true;

            grappleInfo.collider = GrappleUtilities.RaycastResult.collider;
            grappleInfo.offset = GrappleUtilities.RaycastResult.point - grappleInfo.collider.bounds.center;

            RodLength = Vector3.Distance(player.RigidBody.position, GrappleUtilities.RaycastResult.point);
        }
    }
    private void FixedUpdate()
    {
        if (grappleJoint != null)
        {
            grappleJoint.connectedAnchor = GrappleInfo.GrapplePosition;
        }
    }


    public void DisableGrapple()
    {
        grappleActive = false;
        rodLine.enabled = false;
        Destroy(grappleJoint);
    }

    private void LateUpdate()
    {
        if (grappleActive)
        {
            rodLine.SetPosition(0, player.transform.position);
            rodLine.SetPosition(1, grappleInfo.GrapplePosition);
            grapplePoint.position = grappleInfo.GrapplePosition;
        }
    }
}
