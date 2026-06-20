using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "StatsHolder", menuName = "Scriptable Objects/EntityStats/StatsHolder")]
public class StatsHolder : ScriptableObject
{
    public List<StatObject> StatObjects;
}
