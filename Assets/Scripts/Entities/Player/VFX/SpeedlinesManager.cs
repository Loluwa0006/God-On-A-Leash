using UnityEngine;

public class SpeedlinesManager : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] ParticleSystem speedlineParticles;

    [SerializeField] AnimationCurve speedToEmissionRate;

    [SerializeField] float maxSpeedToConsider = 350.0f;
    [SerializeField] float minSpeedToConsider = 100.0f;
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
            var emissionRate = speedToEmissionRate.Evaluate(speedAsPercentage);
            emission.rateOverTime = emissionRate;
        }
    }
}
