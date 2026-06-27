using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "StatObject", menuName = "Scriptable Objects/EntityStats/StatObject/BaseStat")]
public class StatObject : ScriptableObject
{
    [SerializeField, HideIf(nameof(RequiresMultipleIDS))] StatInfluenceType type;

    public StatInfluenceType Type { get => type; }
    [SerializeField, ShowIf(nameof(IsFloatValue))] protected float floatValue;
    [SerializeField, ShowIf(nameof(IsCurveValue))] protected AnimationCurve curveValue;

    [SerializeField, ShowIf(nameof(IsJumpInfoValue))] protected JumpInfo jumpInfoValue;

    [SerializeField, ShowIf(nameof(IsPercentageValue)), Range(0.0f, 1.0f)] protected float percentageValue;

    public object Value
    {
        get
        {
            switch (ValueType)
            {
                case StatValueType.Float:
                    return floatValue;
                case StatValueType.AnimationCurve:
                    return curveValue;
                case StatValueType.JumpInfo:
                    return jumpInfoValue;
                case StatValueType.Percentage:
                    return percentageValue;
                default:
                    Debug.LogWarning($"StatObject {name} has an invalid StatValueType: {ValueType}. Returning floatValue as default.");
                    return floatValue;
            }
        }

    }

    [SerializeField] StatValueType statValueType;

    public StatValueType ValueType { get => statValueType; }

    public bool IsFloatValue () => ValueType == StatValueType.Float;
    public bool IsCurveValue () => ValueType == StatValueType.AnimationCurve;

    public bool IsJumpInfoValue () => ValueType == StatValueType.JumpInfo;

    public bool IsPercentageValue () => ValueType == StatValueType.Percentage;

    public bool RequiresMultipleIDS() => ValueType == StatValueType.JumpInfo;

    public RuntimeStatObject[] CreateRuntimeStats()
    {
        RuntimeStatObject[] stats = new RuntimeStatObject[1];
        switch (ValueType)
        {
            default:
                stats[0] = new RuntimeStatObject(Value, Type, ValueType);
                break;
            case StatValueType.JumpInfo:
                stats = new RuntimeStatObject[3];
                stats[0] = new RuntimeStatObject(jumpInfoValue.JumpGravity, StatInfluenceType.FallSpeed, StatValueType.Float);
                stats[1] = new RuntimeStatObject(jumpInfoValue.FallGravity, StatInfluenceType.FallSpeed, StatValueType.Float);
                stats[2] = new RuntimeStatObject(jumpInfoValue.JumpVelocity, StatInfluenceType.JumpPower, StatValueType.Float);
                break;

        }

        return stats;

    }
}


[System.Serializable]
public struct JumpInfo
{
    public float jumpHeight;
    public float jumpTimeToPeak;
    public float jumpTimeToDecent;
    public float JumpGravity { get => 2.0f * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak); }
    public float FallGravity { get => 2.0f * jumpHeight / (jumpTimeToDecent * jumpTimeToDecent); }
    public float JumpVelocity { get => 2.0f * jumpHeight; }

    public const int JUMP_GRAVITY_ID = 0;
    public const int FALL_GRAVITY_ID = 1;
    public const int JUMP_VELOCITY_ID = 2;

}
