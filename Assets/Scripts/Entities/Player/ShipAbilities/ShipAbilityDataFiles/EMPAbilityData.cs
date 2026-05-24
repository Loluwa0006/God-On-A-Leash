using UnityEngine;

[CreateAssetMenu(fileName = "EMPAbilityData", menuName = "Scriptable Objects/ShipAbilityData/EMPAbilityData")]
public class EMPAbilityData : ShipAbilityData
{
    [SerializeField] float empRange = 85.0f;

    public float EMPRange { get => empRange; }

    [SerializeField] int empActiveFrames = 12;

    public int EMPActiveFrames { get => empActiveFrames; }
}
