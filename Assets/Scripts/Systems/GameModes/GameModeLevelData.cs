using UnityEngine;

[CreateAssetMenu(fileName = "GameModeLevelData", menuName = "Scriptable Objects/GameModeData/GameModeLevelData")]
public class GameModeLevelData : ScriptableObject
{

    [SerializeField] BaseGameMode levelPrefab;

    public BaseGameMode LevelPrefab { get => levelPrefab; }
}
