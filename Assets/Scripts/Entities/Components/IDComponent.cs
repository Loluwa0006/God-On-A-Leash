using UnityEngine;

public class IDComponent : MonoBehaviour
{
    public static int nextID = 0;

    public int ID { get; private set; }

    [System.Serializable]
    public enum IDType
    {
        Player, 
        PlayerProjectile,
        EnemyProjectile,
        ShipAbility,
        Enemy,
        StatsManager,
        Other
    }

    [SerializeField] IDType idType;

    public IDType ID_Type { get => idType; }
    private void Awake()
    {
        ID = nextID;
        nextID++;
    }
}
