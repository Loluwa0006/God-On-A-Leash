using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "ShipAbilityRegister", menuName = "Scriptable Objects/ShipAbilityRegister")]
public class ShipAbilityRegister : ScriptableObject
{
    [SerializeField] List<ShipAbilityIndex> shipAbilityIndex = new();


}
[System.Serializable]
public struct ShipAbilityIndex
{
    public ShipAbilityID ID;
    public BaseShipAbility Prefab;

    public InputManager.BufferableInputs abilityInput;
}

[System.Serializable]
public enum ShipAbilityID
{
    BoostShield,
    LivingRum,
    EMP,
    Chrono,
    SHARK
}