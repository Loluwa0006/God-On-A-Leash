using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameModeData", menuName = "Scriptable Objects/GameModeData/LevelDataHolder")]
public class GameModeData : ScriptableObject
{
    [SerializeField] List<GameModeLevelData> levelData;

    public List<GameModeLevelData> LevelData { get => levelData; }


}
