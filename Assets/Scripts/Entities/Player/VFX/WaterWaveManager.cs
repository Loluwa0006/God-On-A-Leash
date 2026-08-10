using UnityEngine;
using UnityEngine.VFX;

public class WaterWaveManager : MonoBehaviour
{

    [SerializeField] float distanceBeforeWavesShow = 25.0f;
    [SerializeField] float safeMargin = 5.0f;
    [SerializeField] VisualEffect waveLeft;
    [SerializeField] VisualEffect waveRight;

    [SerializeField] PlayerController player;
    [SerializeField] GameObject water;

    Vector3 positionOffset;
    bool emitting = false;
    private void Start()
    {
        positionOffset = waveLeft.transform.localPosition;
        waveLeft.transform.SetParent(null);
        waveRight.transform.SetParent(null);
       // can't be in local space if we want it to lock to the water.
    }


   
    private void FixedUpdate()
    {
        var distance = Mathf.Abs(player.RigidBody.position.y - water.transform.position.y);
        if (distance > distanceBeforeWavesShow)
        {
            emitting = false;
            return;
        }
        Vector3 lateralSpeed = new Vector2(player.RigidBody.linearVelocity.x, player.RigidBody.linearVelocity.z);
        //must be moving fairly fast;
        if (lateralSpeed.magnitude < player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMoveSpeed)) return;
        if (!emitting)
        {
            waveLeft.Play();
            waveRight.Play();
        }

        var lockOnPoint = new Vector3(player.RigidBody.position.x, water.transform.position.y + safeMargin , player.RigidBody.position.z);
        waveLeft.transform.position = new Vector3(lockOnPoint.x, lockOnPoint.y, lockOnPoint.z);
        waveRight.transform.position = new Vector3(lockOnPoint.x, lockOnPoint.y, lockOnPoint.z);
        emitting = true;
    }
   
}
