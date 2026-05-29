using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameModeData", menuName = "Scriptable Objects/GameModeData")]
public class GameModeData : ScriptableObject
{
    [SerializeField] List<GameModeLevelData> levelData;

    public List<GameModeLevelData> LevelData { get => levelData; }


}
