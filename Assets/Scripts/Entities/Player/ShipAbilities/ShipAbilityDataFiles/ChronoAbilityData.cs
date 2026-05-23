using UnityEngine;

[CreateAssetMenu(fileName = "ChronoAbilityData", menuName = "Scriptable Objects/ShipAbilityData/ChronoAbilityData")]
public class ChronoAbilityData : ShipAbilityData
{
    [SerializeField] int abilityDuration = 25;
    public int AbilityDuration => abilityDuration;

    [SerializeField, Range(0, 0.999f)] float timeSlow = 0.85f;

    public float TimeSlow => timeSlow;
}
