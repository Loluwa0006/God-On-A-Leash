using UnityEngine;

[CreateAssetMenu(fileName = "GameModeLevelData", menuName = "Scriptable Objects/GameModeData/LevelData")]
public class GameModeLevelData : ScriptableObject
{

    [SerializeField] BaseGameMode levelPrefab;

    public BaseGameMode LevelPrefab { get => levelPrefab; }
}
