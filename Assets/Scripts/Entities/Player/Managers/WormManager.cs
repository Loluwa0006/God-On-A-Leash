using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    [SerializeField] WormEntity wormPrefab;
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text wormDisplay;

    float wormsRemaining;
    public float WormsRemaining
    {
        get => wormsRemaining ;
        set
        {
            wormsRemaining =
            Mathf.Clamp(value, 0, player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxWorms));
            if (wormDisplay != null) wormDisplay.text = wormsRemaining.ToString();
        }
    }
    Queue<WormEntity> wormPool = new();

    private void Start()
    {
        wormsRemaining = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxWorms);

        for (int i = 0; i < player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxWorms); i++)
        {
            WormEntity newWorm = Instantiate(wormPrefab);
            newWorm.Deactivate();
            wormPool.Enqueue(newWorm);
        }
    }

    public WormEntity GetNewWorm()
    {
        var newWorm = wormPool.Dequeue();
        wormPool.Enqueue(newWorm);
        return newWorm;
    }

}
