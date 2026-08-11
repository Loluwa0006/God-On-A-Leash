using UnityEngine;
using UnityEngine.VFX;

public class WaterWaveManager : MonoBehaviour
{

    [SerializeField] Rigidbody player;
    [SerializeField] Transform water;
    [SerializeField] VisualEffect waveEffect;

    [SerializeField] float rayLength = 25.0f;
    //how far in front of the player the waves should spawn;
    [SerializeField] float moveaheadDistance = 20.0f;
    [SerializeField] LayerMask waveMask;

    bool emitting = false;

    Vector3 baseRotation;

    private void Start()
    {
        waveEffect.transform.SetParent(null);
        waveEffect.Stop();
        baseRotation = waveEffect.GetVector3("Angle");
    }
    private void FixedUpdate()
    {
        CheckForWater();
        
        if (emitting)
        {
            var wavePosition = new Vector3(player.position.x, water.position.y, player.position.z);
            wavePosition += player.transform.forward * moveaheadDistance;
            var velocityRotation = Quaternion.LookRotation(player.linearVelocity.normalized);
            

            waveEffect.transform.SetPositionAndRotation(wavePosition, velocityRotation);
            waveEffect.transform.position = wavePosition;
            waveEffect.SetVector3("Angle", (baseRotation + Quaternion.Euler(velocityRotation.eulerAngles).eulerAngles));
        }
    }

    private void CheckForWater()
    {
        
        bool nowEmitting = Mathf.Abs(player.position.y - water.position.y) <= rayLength;
        
        //was emitting, no longer emitting , turn off
        if (emitting && !nowEmitting)
        {
            waveEffect.Stop();
        }
        //wasn't emitting, now emitting, turn on
        else if (!emitting && nowEmitting)
        {
            waveEffect.Play();
        }
        emitting = nowEmitting;
        
    }

}
