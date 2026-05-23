using UnityEngine;

[CreateAssetMenu(fileName = "ShipAbilityData", menuName = "Scriptable Objects/ShipAbilityData")]
public class ShipAbilityData : ScriptableObject
{
    [SerializeField] int abilityCost;

    public int AbilityCost { get => abilityCost; }

    [SerializeField] Texture abilityIcon;

    public Texture AbilityIcon { get => abilityIcon; }
}

[System.Serializable]
public enum ShipAbilityRegistry
{
    BoostShield,
    LivingRum,
    EMP,
    Chrono,
    SHARK
}