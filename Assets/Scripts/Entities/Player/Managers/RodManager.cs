using TMPro;
using UnityEngine;

public class RodManager : BaseEntity
{
    [SerializeField] LineRenderer rodLine;
    [SerializeField] LayerMask grappleMask;
    [SerializeField] PlayerController player;
    [SerializeField] Transform GrappleLineStartPoint;
    [SerializeField] Transform grapplePoint;
    [SerializeField] TMP_Text rodLengthDisplay;

    [Header("Grapple Animations")]
    //ampitiude 
    [SerializeField] float waveLength = 0.6f;

    //number of flu- im not gonna try spelling it
    [SerializeField] int waveCount = 8;
    // time before waves become straight again
    [SerializeField] float waveDuration = 0.2f;

    float elaspedGrappleTime = 0.0f;
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

    public float RodLengthPercentage
    { 
        get => RodLength / player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxRodRange);
    }
    public override void Initialize()
    {
        base.Initialize();
        rodLine.positionCount = waveCount;
        DisableGrapple();
        RodLength = 0.0f;
    }

    public void StartSwing()
    {
        if (GrappleUtilities.RaycastResult.collider != null)
        {
            EnableGrapple();
            grappleJoint.massScale = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSwingMassScale);
            grappleJoint.spring = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSpring);
            grappleJoint.damper = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodDamper);

            grappleJoint.maxDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMaxDistanceWithNoSpring);
            grappleJoint.minDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMinDistanceWithNoSpring);
        }
    }

    public void StartDash()
    {
        if (GrappleUtilities.RaycastResult.collider != null)
        {
            EnableGrapple();

            grappleJoint.massScale = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSwingMassScale);
            grappleJoint.spring = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodSpringWhileDashing);
            grappleJoint.damper = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodDamperWhileDashing);

            grappleJoint.maxDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMaxDistanceWithNoSpringWhileDashing);
            grappleJoint.minDistance = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodMinDistanceWithNoSpringWhileDashing);
        }
    }


    private void FixedUpdate()
    {
        if (grappleJoint != null)
        {
            grappleJoint.connectedAnchor = GrappleInfo.GrapplePosition;
        }
    }

    void EnableGrapple()
    {
        elaspedGrappleTime = 0.0f;
        grappleInfo.collider = GrappleUtilities.RaycastResult.collider;
        grappleInfo.offset = GrappleUtilities.RaycastResult.point - grappleInfo.collider.bounds.center;

        grappleJoint = player.gameObject.AddComponent<SpringJoint>();
        grappleJoint.autoConfigureConnectedAnchor = false;
        grappleJoint.connectedAnchor = grappleInfo.GrapplePosition;

        grappleActive = true;
        rodLine.enabled = true;

        RodLength = Vector3.Distance(player.RigidBody.position, GrappleUtilities.RaycastResult.point);
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
            Vector3 startPosition = GrappleLineStartPoint.position;
            Vector3 endPosition = grappleInfo.GrapplePosition;
            Vector3 direction = (endPosition - startPosition).normalized;
            float timeElaspedAsPercent = elaspedGrappleTime/waveDuration;
            if (timeElaspedAsPercent > 0.999f) timeElaspedAsPercent = 1.0f;
            float wavePower = ( 1.0f - timeElaspedAsPercent);

            for (int i = 0; i < waveCount; i++)
            {
                float progress = i / ((float)waveCount - 1);
                Vector3 pointPosition = Vector3.Lerp(startPosition, endPosition, progress);

                Vector3 waveDirection = Vector3.Cross(Vector3.Cross(direction, Vector3.up), direction);
                float randomSample = Random.Range(0, 1000000);
                float sineStuff = Mathf.Sin(randomSample * Mathf.PI * 2) * waveLength * wavePower;
                pointPosition += waveDirection * sineStuff;
                rodLine.SetPosition(i, pointPosition);
            }
            grapplePoint.position = grappleInfo.GrapplePosition;
        }
    }

    private void Update()
    {
        if (grappleActive)
        {
            elaspedGrappleTime += Time.deltaTime;
        }
    }
}
