using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]  GameModeData selectedGameMode;

    [SerializeField] List<GameModeData> registryEntries = new();
}
