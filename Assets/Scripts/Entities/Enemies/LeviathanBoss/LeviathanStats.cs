using UnityEngine;

[CreateAssetMenu(fileName = "LeviathanStats", menuName = "Scriptable Objects/EntityStats/LeviathanStats")]
public class LeviathanStats : ScriptableObject
{
    #region Movement
    [SerializeField] float moveSpeed = 5.0f;
    public float MoveSpeed { get => moveSpeed; }

    [SerializeField] int minMoveDuration = 42;

    public int MinMoveDuration { get => minMoveDuration; }

    [SerializeField] int maxMoveDuration = 120;

    public int MaxMoveDuration { get => maxMoveDuration; }
    #endregion
}