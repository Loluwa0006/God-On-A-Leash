using UnityEngine;

public class SpeedlinesManager : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] ParticleSystem speedlineParticles;

    [SerializeField] AnimationCurve speedToEmissionRate;

    [SerializeField] float maxSpeedToConsider = 350.0f;
    [SerializeField] float minSpeedToConsider = 100.0f;
    [SerializeField] float minEmissionRate = 25.0f;
    [SerializeField] float maxEmissionRate = 50.0f;
    public void FixedUpdate()
    {
        Vector2 lateralSpeed = new (player.RigidBody.linearVelocity.x, player.RigidBody.linearVelocity.z);
        var emission = speedlineParticles.emission;
        if (lateralSpeed.magnitude < minSpeedToConsider)
        {
            emission.rateOverTime = 0.0f;
        }
        else
        {
            var speedAsPercentage = Mathf.Clamp01((lateralSpeed.magnitude - minSpeedToConsider) / (maxSpeedToConsider - minSpeedToConsider));
            var emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, speedToEmissionRate.Evaluate(speedAsPercentage));
            emission.rateOverTime = emissionRate;
            speedlineParticles.transform.rotation = Quaternion.LookRotation(-player.RigidBody.linearVelocity.normalized, Vector3.up);
        }
    }
}
