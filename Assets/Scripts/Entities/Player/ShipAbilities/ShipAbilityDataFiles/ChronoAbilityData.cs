using UnityEngine;

[CreateAssetMenu(fileName = "ChronoAbilityData", menuName = "Scriptable Objects/ShipAbilityData/ChronoAbilityData")]
public class ChronoAbilityData : ShipAbilityData
{
    [SerializeField] int abilityDuration = 25;

    public int AbilityDuration => abilityDuration;
}
