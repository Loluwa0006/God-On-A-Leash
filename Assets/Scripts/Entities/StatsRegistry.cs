using UnityEngine;

[CreateAssetMenu(fileName = "StatsRegistry", menuName = "Scriptable Objects/StatsRegistry")]
public class StatsRegistry : ScriptableObject
{

    [SerializeField] PlayerStats playerStats;
    public PlayerStats PlayerStats => playerStats;

    [SerializeField] LeviathanStats leviathanStats;

    public LeviathanStats LeviathanStats => leviathanStats;


}
